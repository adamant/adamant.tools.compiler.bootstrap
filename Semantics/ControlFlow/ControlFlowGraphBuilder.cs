using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Literals;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow.Graph;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Statements;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Statements.LValues;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow
{
    public class ControlFlowGraphBuilder
    {
        public void BuildGraph([NotNull][ItemNotNull] IEnumerable<FunctionDeclarationAnalysis> functions)
        {
            Requires.NotNull(nameof(functions), functions);

            foreach (var function in functions.Where(f => !f.Diagnostics.Any() && f.Syntax.Body != null))
                BuildGraph(function);
        }

        private void BuildGraph([NotNull] FunctionDeclarationAnalysis function)
        {
            Requires.NotNull(nameof(function), function);

            var cfg = new ControlFlowGraph();
            function.ControlFlow = cfg;

            // Temp Variable for return
            cfg.Let(function.ReturnType.AssertKnown());
            foreach (var parameter in function.Parameters)
                cfg.AddParameter(parameter.MutableBinding, parameter.Type.AssertKnown(), parameter.Name.Name.Text);

            var blocks = new Dictionary<SyntaxNode, BasicBlock>();
            var entryBlock = cfg.EntryBlock;
            blocks.Add(function.Syntax.Body, entryBlock);
            var currentBlock = entryBlock;
            foreach (var statement in function.Statements)
                Convert(cfg, statement, currentBlock);

            // Generate the implicit return statement
            if (currentBlock.Number == 0 && currentBlock.EndStatement == null)
                currentBlock.End(new ReturnStatement());
        }

        [NotNull]
        private VariableReference LookupVariable([NotNull] ControlFlowGraph cfg, [NotNull] string name)
        {
            Requires.NotNull(nameof(name), name);
            return cfg.VariableDeclarations.Single(v => v.Name == name).AssertNotNull().Reference;
        }

        private void Convert([NotNull] ControlFlowGraph cfg, [NotNull] StatementAnalysis statement, [NotNull] BasicBlock currentBlock)
        {
            Requires.NotNull(nameof(cfg), cfg);
            Requires.NotNull(nameof(statement), statement);
            Requires.NotNull(nameof(currentBlock), currentBlock);

            switch (statement)
            {
                case VariableDeclarationStatementAnalysis variableDeclaration:
                    var variable = cfg.AddVariable(variableDeclaration.MutableBinding,
                        variableDeclaration.Type.AssertKnown(),
                        variableDeclaration.Name.Name.Text);
                    if (variableDeclaration.Initializer != null)
                        ConvertAssignment(cfg, variable.Reference, variableDeclaration.Initializer, currentBlock);
                    break;

                case ExpressionStatementAnalysis expressionStatement:
                    Convert(cfg, expressionStatement.Expression, currentBlock);
                    break;

                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }

        private static bool IsOwned([NotNull] VariableDeclarationStatementAnalysis declaration)
        {
            Requires.NotNull(nameof(declaration), declaration);
            if (declaration.Type is LifetimeType type)
                return type.IsOwned;

            return false;
        }

        private void Convert([NotNull] ControlFlowGraph cfg, [NotNull] ExpressionAnalysis expression, [NotNull] BasicBlock currentBlock)
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
                        case EqualsToken _:
                            var lvalue = ConvertToLValue(cfg, binaryOperatorExpression.LeftOperand);
                            ConvertAssignment(cfg, lvalue, binaryOperatorExpression.RightOperand, currentBlock);
                            break;
                        default:
                            // Could be side effects possibly.
                            var temp = cfg.Let(binaryOperatorExpression.Type.AssertKnown());
                            ConvertAssignment(cfg, temp.Reference, expression, currentBlock);
                            break;
                    }
                    break;
                case ReturnExpressionAnalysis returnExpression:
                    if (returnExpression.ReturnExpression != null)
                        ConvertAssignment(cfg, cfg.ReturnVariable.Reference, returnExpression.ReturnExpression, currentBlock);
                    currentBlock.End(new ReturnStatement());
                    break;
                case BlockAnalysis block:
                    foreach (var statementInBlock in block.Statements)
                        Convert(cfg, statementInBlock, currentBlock);

                    // Now we need to delete any owned variables
                    foreach (var variableDeclaration in block.Statements.OfType<VariableDeclarationStatementAnalysis>().Where(IsOwned))
                        currentBlock.Add(new DeleteStatement(LookupVariable(cfg, variableDeclaration.Name.Name.Text).VariableNumber, block.Syntax.CloseBrace.Span));

                    break;
                case ForeachExpressionAnalysis @foreach:
                    // TODO actually convert the expression
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private void ConvertAssignment(
            [NotNull] ControlFlowGraph cfg,
            [NotNull] LValue lvalue,
            [NotNull] ExpressionAnalysis value,
            [NotNull] BasicBlock currentBlock)
        {
            Requires.NotNull(nameof(cfg), cfg);
            Requires.NotNull(nameof(lvalue), lvalue);
            Requires.NotNull(nameof(value), value);
            Requires.NotNull(nameof(currentBlock), currentBlock);

            switch (value)
            {
                case NewObjectExpressionAnalysis newObjectExpression:
                    var args = newObjectExpression.Arguments.Select(a => ConvertToLValue(cfg, a.Value));
                    currentBlock.Add(new NewObjectStatement(lvalue, newObjectExpression.Type.AssertKnown(), args));
                    break;
                case IdentifierNameAnalysis identifier:
                    currentBlock.Add(new AssignmentStatement(lvalue, LookupVariable(cfg, identifier.Name.AssertNotNull())));
                    break;
                case BinaryOperatorExpressionAnalysis binaryOperator:
                    ConvertOperator(cfg, lvalue, binaryOperator, currentBlock);
                    break;
                case IntegerLiteralExpressionAnalysis v:
                    currentBlock.Add(new IntegerLiteralStatement(lvalue, v.Value));
                    break;
                case IfExpressionAnalysis ifExpression:
                    Convert(cfg, ifExpression.Condition, currentBlock);
                    // TODO assign the result into the temp, branch and execute then or else, assign result
                    break;
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }

        private void ConvertOperator(
            [NotNull] ControlFlowGraph cfg,
            [NotNull] LValue lvalue,
            [NotNull] BinaryOperatorExpressionAnalysis binaryOperator,
            [NotNull] BasicBlock currentBlock)
        {
            switch (binaryOperator.Operator)
            {
                case PlusToken _:
                    currentBlock.Add(new AddStatement(lvalue,
                        ConvertToLValue(cfg, binaryOperator.LeftOperand),
                        ConvertToLValue(cfg, binaryOperator.RightOperand)));
                    break;
                case LessThanToken _:
                case LessThanOrEqualToken _:
                case GreaterThanToken _:
                case GreaterThanOrEqualToken _:
                    // TODO generate the correct statement
                    break;
                default:
                    throw NonExhaustiveMatchException.For(binaryOperator.Operator);
            }
        }

        [NotNull]
        private LValue ConvertToLValue([NotNull] ControlFlowGraph cfg, [NotNull] ExpressionAnalysis value)
        {
            Requires.NotNull(nameof(value), value);
            switch (value)
            {
                case IdentifierNameAnalysis identifier:
                    return LookupVariable(cfg, identifier.Name.AssertNotNull());
                //case VariableExpression variableExpression:
                //    return LookupVariable(variableExpression.Name);
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }
    }
}
