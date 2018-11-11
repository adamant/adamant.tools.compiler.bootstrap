using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses;
using Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers
{
    public class ControlFlowGraphBuilder
    {
        public void BuildGraph([NotNull][ItemNotNull] IEnumerable<FunctionDeclarationAnalysis> functions)
        {
            Requires.NotNull(nameof(functions), functions);

            foreach (var function in functions.Where(f => !f.Diagnostics.Any() // It has errors, can't generate intermediate code
                                                          && f.Syntax.Body != null // It is abstract
                                                          && !f.IsGeneric)) // It is not generic, generic functions need monomorphized
                BuildGraph(function);
        }

        private void BuildGraph([NotNull] FunctionDeclarationAnalysis function)
        {
            Requires.NotNull(nameof(function), function);

            var cfg = function.CreateControlFlowGraph();

            // Temp Variable for return
            cfg.Let(function.ReturnType.AssertResolved());
            foreach (var parameter in function.Parameters)
                cfg.AddParameter(parameter.MutableBinding, parameter.Type.AssertResolved(), parameter.Name.UnqualifiedName.Text);

            var blocks = new Dictionary<NonTerminal, BasicBlock>();
            var entryBlock = cfg.EntryBlock;
            blocks.Add(function.Syntax.Body, entryBlock);
            var currentBlock = entryBlock;
            foreach (var statement in function.Statements)
                ConvertStatementAnalysisToStatement(cfg, currentBlock, statement);

            // Generate the implicit return statement
            if (currentBlock.Number == 0 && currentBlock.Terminator == null)
                currentBlock.End(new ReturnTerminator());
        }

        [NotNull]
        private VariableReference LookupVariable([NotNull] ControlFlowGraph cfg, [NotNull] string name)
        {
            Requires.NotNull(nameof(name), name);
            return cfg.VariableDeclarations.Single(v => v.Name == name).NotNull().Reference;
        }

        private void ConvertStatementAnalysisToStatement(
            [NotNull] ControlFlowGraph cfg,
            [NotNull] BasicBlock currentBlock,
            [NotNull] StatementAnalysis statement)
        {
            Requires.NotNull(nameof(cfg), cfg);
            Requires.NotNull(nameof(statement), statement);
            Requires.NotNull(nameof(currentBlock), currentBlock);

            switch (statement)
            {
                case VariableDeclarationStatementAnalysis variableDeclaration:
                    var variable = cfg.AddVariable(variableDeclaration.MutableBinding,
                        variableDeclaration.Type.AssertResolved(),
                        variableDeclaration.Name.UnqualifiedName.Text);
                    if (variableDeclaration.Initializer != null)
                        ConvertToAssignmentStatement(cfg, variable.Reference, variableDeclaration.Initializer, currentBlock);
                    break;

                case ExpressionStatementAnalysis expressionStatement:
                    ConvertExpressionAnalysisToStatement(cfg, currentBlock, expressionStatement.Expression);
                    break;

                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }

        private static bool IsOwned([NotNull] VariableDeclarationStatementAnalysis declaration)
        {
            Requires.NotNull(nameof(declaration), declaration);
            if (declaration.Type.AssertResolved() is LifetimeType type)
                return type.IsOwned;

            return false;
        }

        private void ConvertExpressionAnalysisToStatement(
            [NotNull] ControlFlowGraph cfg,
            [NotNull] BasicBlock currentBlock,
            [NotNull] ExpressionAnalysis expression)
        {
            Requires.NotNull(nameof(cfg), cfg);
            Requires.NotNull(nameof(expression), expression);
            Requires.NotNull(nameof(currentBlock), currentBlock);

            switch (expression)
            {
                case IdentifierNameAnalysis _:
                    // Ignore, reading from variable does nothing.
                    break;
                case BinaryOperatorExpressionAnalysis binaryOperatorExpression:
                    switch (binaryOperatorExpression.Operator)
                    {
                        case IEqualsToken _:
                            var lvalue = ConvertToLValue(cfg, binaryOperatorExpression.LeftOperand);
                            ConvertToAssignmentStatement(cfg, lvalue, binaryOperatorExpression.RightOperand, currentBlock);
                            break;
                        default:
                            // Could be side effects possibly.
                            var temp = cfg.Let(binaryOperatorExpression.Type.AssertResolved());
                            ConvertToAssignmentStatement(cfg, temp.Reference, expression, currentBlock);
                            break;
                    }
                    break;
                case ReturnExpressionAnalysis returnExpression:
                    if (returnExpression.ReturnExpression != null)
                        ConvertToAssignmentStatement(cfg, cfg.ReturnVariable.Reference, returnExpression.ReturnExpression, currentBlock);
                    currentBlock.End(new ReturnTerminator());
                    break;
                case BlockAnalysis block:
                    foreach (var statementInBlock in block.Statements)
                        ConvertStatementAnalysisToStatement(cfg, currentBlock, statementInBlock);

                    // Now we need to delete any owned variables
                    foreach (var variableDeclaration in block.Statements.OfType<VariableDeclarationStatementAnalysis>().Where(IsOwned))
                        currentBlock.Add(new DeleteStatement(LookupVariable(cfg, variableDeclaration.Name.UnqualifiedName.Text).VariableNumber, new TextSpan(block.Syntax.Span.End, 0)));

                    break;
                case ForeachExpressionAnalysis @foreach:
                    // TODO actually convert the expression
                    break;
                case WhileExpressionAnalysis @while:
                    // TODO actually convert the expression
                    break;
                case LoopExpressionAnalysis loop:
                    // TODO actually convert the expression
                    break;
                case UnsafeExpressionAnalysis unsafeExpression:
                    ConvertExpressionAnalysisToStatement(cfg, currentBlock, unsafeExpression.Expression);
                    break;
                case InvocationAnalysis invocation:
                    // TODO actually convert the expression
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private void ConvertToAssignmentStatement(
            [NotNull] ControlFlowGraph cfg,
            [NotNull] Place place,
            [NotNull] ExpressionAnalysis value,
            [NotNull] BasicBlock currentBlock)
        {
            Requires.NotNull(nameof(cfg), cfg);
            Requires.NotNull(nameof(place), place);
            Requires.NotNull(nameof(value), value);
            Requires.NotNull(nameof(currentBlock), currentBlock);

            switch (value)
            {
                case NewObjectExpressionAnalysis newObjectExpression:
                    var args = newObjectExpression.Arguments.Select(a => ConvertToLValue(cfg, a.Value));
                    currentBlock.Add(new NewObjectStatement(place, newObjectExpression.Type.AssertResolved(), args));
                    break;
                case IdentifierNameAnalysis identifier:
                    currentBlock.Add(new AssignmentStatement(place, new CopyPlace(LookupVariable(cfg, identifier.Name.NotNull()))));
                    break;
                case BinaryOperatorExpressionAnalysis binaryOperator:
                    ConvertOperator(cfg, place, binaryOperator, currentBlock);
                    break;
                case IntegerLiteralExpressionAnalysis v:
                    var constant = new IntegerConstant(v.Value, v.Type);
                    currentBlock.Add(new AssignmentStatement(place, constant));
                    break;
                case IfExpressionAnalysis ifExpression:
                    ConvertExpressionAnalysisToStatement(cfg, currentBlock, ifExpression.Condition);
                    // TODO assign the result into the temp, branch and execute then or else, assign result
                    break;
                case UnsafeExpressionAnalysis unsafeExpression:
                    ConvertExpressionAnalysisToStatement(cfg, currentBlock, unsafeExpression.Expression);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }

        private void ConvertOperator(
            [NotNull] ControlFlowGraph cfg,
            [NotNull] Place lvalue,
            [NotNull] BinaryOperatorExpressionAnalysis binaryOperator,
            [NotNull] BasicBlock currentBlock)
        {
            switch (binaryOperator.Operator)
            {
                //case PlusToken _:
                //    currentBlock.Add(new AddStatement(lvalue,
                //        ConvertToLValue(cfg, binaryOperator.LeftOperand),
                //        ConvertToLValue(cfg, binaryOperator.RightOperand)));
                //    break;
                case ILessThanToken _:
                case ILessThanOrEqualToken _:
                case IGreaterThanToken _:
                case IGreaterThanOrEqualToken _:
                    // TODO generate the correct statement
                    break;
                case IAsKeywordToken _:
                    ConvertExpressionAnalysisToStatement(cfg, currentBlock, binaryOperator.LeftOperand);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(binaryOperator.Operator);
            }
        }

        [NotNull]
        private Place ConvertToLValue([NotNull] ControlFlowGraph cfg, [NotNull] ExpressionAnalysis value)
        {
            Requires.NotNull(nameof(value), value);
            switch (value)
            {
                case IdentifierNameAnalysis identifier:
                    return LookupVariable(cfg, identifier.Name.NotNull());
                //case VariableExpression variableExpression:
                //    return LookupVariable(variableExpression.Name);
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }
    }
}
