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
        public static void BuildGraphs([NotNull, ItemNotNull] IEnumerable<FunctionDeclarationAnalysis> functions)
        {
            Requires.NotNull(nameof(functions), functions);

            foreach (var function in functions.Where(f => !f.Diagnostics.Any() // It has errors, can't generate intermediate code
                                                          && f.Syntax.Body != null // It is abstract
                                                          && !f.IsGeneric)) // It is not generic, generic functions need monomorphized
            {
                var builder = new ControlFlowGraphBuilder();
                builder.BuildGraph(function);
            }
        }

        [NotNull, ItemNotNull] private readonly List<LocalVariableDeclaration> variables = new List<LocalVariableDeclaration>();
        [NotNull] private LocalVariableDeclaration ReturnVariable => variables.First().NotNull();
        [NotNull, ItemNotNull] private readonly List<BasicBlock> blocks = new List<BasicBlock>();

        [NotNull]
        public LocalVariableDeclaration AddParameter(bool mutableBinding, [NotNull] DataType type, [CanBeNull] string name)
        {
            var variable = new LocalVariableDeclaration(true, mutableBinding, type, variables.Count)
            {
                Name = name
            };
            variables.Add(variable);
            return variable;
        }

        [NotNull]
        public LocalVariableDeclaration AddVariable(bool mutableBinding, [NotNull] DataType type, [CanBeNull] string name = null)
        {
            var variable = new LocalVariableDeclaration(false, mutableBinding, type, variables.Count)
            {
                Name = name
            };
            variables.Add(variable);
            return variable;
        }

        [NotNull]
        public LocalVariableDeclaration Let([NotNull] DataType type)
        {
            return AddVariable(false, type);
        }

        [NotNull]
        public LocalVariableDeclaration Var([NotNull] DataType type)
        {
            return AddVariable(true, type);
        }

        [NotNull]
        public BasicBlock AddBlock()
        {
            var block = new BasicBlock(blocks.Count);
            blocks.Add(block);
            return block;
        }

        private void BuildGraph([NotNull] FunctionDeclarationAnalysis function)
        {
            Requires.NotNull(nameof(function), function);

            // Temp Variable for return
            Let(function.ReturnType.AssertResolved());
            foreach (var parameter in function.Parameters)
                AddParameter(parameter.MutableBinding, parameter.Type.AssertResolved(), parameter.Name.UnqualifiedName.Text);

            var blockMap = new Dictionary<NonTerminal, BasicBlock>();
            var entryBlock = AddBlock();
            blockMap.Add(function.Syntax.Body, entryBlock);
            var currentBlock = entryBlock;
            foreach (var statement in function.Statements)
                ConvertStatementAnalysisToStatement(currentBlock, statement);

            // Generate the implicit return statement
            if (currentBlock.Number == 0 && currentBlock.Terminator == null)
                currentBlock.End(new ReturnTerminator());

            function.ControlFlow = new ControlFlowGraph(variables, blocks);
        }

        [NotNull]
        private VariableReference LookupVariable([NotNull] string name)
        {
            return variables.Single(v => v.Name == name).NotNull().Reference;
        }

        private void ConvertStatementAnalysisToStatement(
            [NotNull] BasicBlock currentBlock,
            [NotNull] StatementAnalysis statement)
        {
            switch (statement)
            {
                case VariableDeclarationStatementAnalysis variableDeclaration:
                    var variable = AddVariable(variableDeclaration.MutableBinding,
                        variableDeclaration.Type.AssertResolved(),
                        variableDeclaration.Name.UnqualifiedName.Text);
                    if (variableDeclaration.Initializer != null)
                        ConvertToAssignmentStatement(variable.Reference, variableDeclaration.Initializer, currentBlock);
                    break;

                case ExpressionStatementAnalysis expressionStatement:
                    ConvertExpressionAnalysisToStatement(currentBlock, expressionStatement.Expression);
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
            [NotNull] BasicBlock currentBlock,
            [NotNull] ExpressionAnalysis expression)
        {
            switch (expression)
            {
                case IdentifierNameAnalysis _:
                    // Ignore, reading from variable does nothing.
                    break;
                case BinaryOperatorExpressionAnalysis binaryOperatorExpression:
                    switch (binaryOperatorExpression.Operator)
                    {
                        case IEqualsToken _:
                            var lvalue = ConvertToLValue(binaryOperatorExpression.LeftOperand);
                            ConvertToAssignmentStatement(lvalue, binaryOperatorExpression.RightOperand, currentBlock);
                            break;
                        default:
                            // Could be side effects possibly.
                            var temp = Let(binaryOperatorExpression.Type.AssertResolved());
                            ConvertToAssignmentStatement(temp.Reference, expression, currentBlock);
                            break;
                    }
                    break;
                case ReturnExpressionAnalysis returnExpression:
                    if (returnExpression.ReturnExpression != null)
                        ConvertToAssignmentStatement(ReturnVariable.Reference, returnExpression.ReturnExpression, currentBlock);
                    currentBlock.End(new ReturnTerminator());
                    break;
                case BlockAnalysis block:
                    foreach (var statementInBlock in block.Statements)
                        ConvertStatementAnalysisToStatement(currentBlock, statementInBlock);

                    // Now we need to delete any owned variables
                    foreach (var variableDeclaration in block.Statements.OfType<VariableDeclarationStatementAnalysis>().Where(IsOwned))
                        currentBlock.Add(new DeleteStatement(LookupVariable(variableDeclaration.Name.UnqualifiedName.Text).VariableNumber, new TextSpan(block.Syntax.Span.End, 0)));

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
                    ConvertExpressionAnalysisToStatement(currentBlock, unsafeExpression.Expression);
                    break;
                case InvocationAnalysis invocation:
                    // TODO actually convert the expression
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private void ConvertToAssignmentStatement(
            [NotNull] Place place,
            [NotNull] ExpressionAnalysis value,
            [NotNull] BasicBlock currentBlock)
        {
            switch (value)
            {
                case NewObjectExpressionAnalysis newObjectExpression:
                    var args = newObjectExpression.Arguments.Select(a => ConvertToLValue(a.Value));
                    currentBlock.Add(new NewObjectStatement(place, newObjectExpression.Type.AssertResolved(), args));
                    break;
                case IdentifierNameAnalysis identifier:
                    currentBlock.Add(new AssignmentStatement(place, new CopyPlace(LookupVariable(identifier.Name.NotNull()))));
                    break;
                case BinaryOperatorExpressionAnalysis binaryOperator:
                    ConvertOperator(place, binaryOperator, currentBlock);
                    break;
                case IntegerLiteralExpressionAnalysis v:
                    var constant = new IntegerConstant(v.Value, v.Type);
                    currentBlock.Add(new AssignmentStatement(place, constant));
                    break;
                case IfExpressionAnalysis ifExpression:
                    ConvertExpressionAnalysisToStatement(currentBlock, ifExpression.Condition);
                    // TODO assign the result into the temp, branch and execute then or else, assign result
                    break;
                case UnsafeExpressionAnalysis unsafeExpression:
                    ConvertExpressionAnalysisToStatement(currentBlock, unsafeExpression.Expression);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }

        private void ConvertOperator(
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
                    ConvertExpressionAnalysisToStatement(currentBlock, binaryOperator.LeftOperand);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(binaryOperator.Operator);
            }
        }

        [NotNull]
        private Place ConvertToLValue([NotNull] ExpressionAnalysis value)
        {
            Requires.NotNull(nameof(value), value);
            switch (value)
            {
                case IdentifierNameAnalysis identifier:
                    return LookupVariable(identifier.Name.NotNull());
                //case VariableExpression variableExpression:
                //    return LookupVariable(variableExpression.Name);
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }
    }
}
