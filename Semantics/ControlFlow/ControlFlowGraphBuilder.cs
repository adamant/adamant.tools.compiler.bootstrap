using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow.Graph;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Statements;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Statements.LValues;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow
{
    public class ControlFlowGraphBuilder
    {
        public void BuildGraph([NotNull][ItemNotNull] IEnumerable<FunctionDeclarationAnalysis> functions)
        {
            Requires.NotNull(nameof(functions), functions);

            foreach (var function in functions)
                BuildGraph(function);
        }

        private void BuildGraph([NotNull] FunctionDeclarationAnalysis function)
        {
            Requires.NotNull(nameof(function), function);

            var cfg = function.Semantics.ControlFlow;

            // Temp Variable for return
            cfg.Let(function.Semantics.ReturnType);
            foreach (var parameter in function.Semantics.Parameters)
                cfg.AddVariable(parameter.MutableBinding, parameter.Type, parameter.Name);

            var blocks = new Dictionary<SyntaxNode, BasicBlock>();
            var entryBlock = cfg.EntryBlock;
            blocks.Add(function.Syntax.Body, entryBlock);
            var currentBlock = entryBlock;
            foreach (var statement in function.Syntax.Body.Statements)
                Convert(cfg, statement, currentBlock);

            if (!function.Syntax.Body.Statements.Any())
                currentBlock.End(new ReturnStatement());
        }

        [NotNull]
        private VariableReference LookupVariable([NotNull] ControlFlowGraph cfg, [NotNull] string name)
        {
            Requires.NotNull(nameof(name), name);
            return cfg.VariableDeclarations.Single(v => v.Name == name).AssertNotNull().Reference;
        }

        private void Convert([NotNull] ControlFlowGraph cfg, [NotNull] StatementSyntax statement, [NotNull] BasicBlock currentBlock)
        {
            Requires.NotNull(nameof(cfg), cfg);
            Requires.NotNull(nameof(statement), statement);
            Requires.NotNull(nameof(currentBlock), currentBlock);

            switch (statement)
            {
                case VariableDeclarationStatementSyntax variableDeclaration:
                    //                    var variable = il.AddVariable(variableDeclaration.MutableBinding,
                    //                        variableDeclaration.Type,
                    //                        variableDeclaration.Name);
                    //                    if (variableDeclaration.Initializer != null)
                    //                        ConvertAssignment(variable.Reference, variableDeclaration.Initializer);
                    break;

                case ExpressionStatementSyntax expressionStatement:
                    Convert(cfg, expressionStatement.Expression, currentBlock);
                    break;

                //                case Block block:
                //                    foreach (var statementInBlock in block.Statements)
                //                        Convert(statementInBlock);

                //                    // Now we need to delete any owned variables
                //                    foreach (var variableDeclaration in block.Statements.OfType<VariableDeclarationStatement>().Where(IsOwned))
                //                        currentBlock.Add(new DeleteStatement(LookupVariable(variableDeclaration.Name).VariableNumber));

                //                    break;

                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }

        //private static bool IsOwned([NotNull] VariableDeclarationStatement declaration)
        //        {
        //            Requires.NotNull(nameof(declaration), declaration);
        //            if (declaration.Type is LifetimeType type)
        //                return type.IsOwned;

        //            return false;
        //        }

        private void Convert([NotNull] ControlFlowGraph cfg, [NotNull] ExpressionSyntax expression, [NotNull] BasicBlock currentBlock)
        {
            Requires.NotNull(nameof(cfg), cfg);
            Requires.NotNull(nameof(expression), expression);
            Requires.NotNull(nameof(currentBlock), currentBlock);

            switch (expression)
            {
                case IdentifierNameSyntax _:
                    // Ignore, reading from variable does nothing.
                    break;
                case BinaryOperatorExpressionSyntax _:
                    // Could be side effects possibly.
                    var temp = cfg.Let(DataType.Unknown);
                    ConvertAssignment(cfg, temp.Reference, expression, currentBlock);
                    break;
                case ReturnExpressionSyntax returnExpression:
                    if (returnExpression.Expression != null)
                        ConvertAssignment(cfg, cfg.ReturnVariable.Reference, returnExpression.Expression, currentBlock);
                    currentBlock.End(new ReturnStatement());
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private void ConvertAssignment(
            [NotNull] ControlFlowGraph cfg,
            [NotNull] LValue lvalue,
            [NotNull] ExpressionSyntax value,
            [NotNull] BasicBlock currentBlock)
        {
            Requires.NotNull(nameof(cfg), cfg);
            Requires.NotNull(nameof(lvalue), lvalue);
            Requires.NotNull(nameof(value), value);
            Requires.NotNull(nameof(currentBlock), currentBlock);

            switch (value)
            {
                //                case NewObjectExpression newObjectExpression:
                //                    var args = newObjectExpression.Arguments.Select(ConvertToLValue);
                //                    currentBlock.Add(new NewObjectStatement(lvalue, newObjectExpression.Type, args));
                //                    break;
                case IdentifierNameSyntax identifier:
                    currentBlock.Add(new AssignmentStatement(lvalue, LookupVariable(cfg, identifier.Name?.Value)));
                    break;
                case BinaryOperatorExpressionSyntax binaryOperator:
                    ConvertOperator(cfg, lvalue, binaryOperator, currentBlock);
                    break;
                case IntegerLiteralExpressionSyntax v:
                    currentBlock.Add(new IntegerLiteralStatement(lvalue, v.IntegerLiteral.Value));
                    break;
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }

        private void ConvertOperator(
            [NotNull] ControlFlowGraph cfg,
            [NotNull] LValue lvalue,
            [NotNull] BinaryOperatorExpressionSyntax binaryOperator,
            [NotNull] BasicBlock currentBlock)
        {
            switch (binaryOperator.Operator)
            {
                case PlusToken _:
                    currentBlock.Add(new AddStatement(lvalue,
                        ConvertToLValue(cfg, binaryOperator.LeftOperand),
                        ConvertToLValue(cfg, binaryOperator.RightOperand)));
                    break;
                default:
                    throw NonExhaustiveMatchException.For(binaryOperator.Operator);
            }
        }

        [NotNull]
        private LValue ConvertToLValue([NotNull] ControlFlowGraph cfg, [NotNull] ExpressionSyntax value)
        {
            Requires.NotNull(nameof(value), value);
            switch (value)
            {
                case IdentifierNameSyntax identifier:
                    return LookupVariable(cfg, identifier.Name.Value);
                //case VariableExpression variableExpression:
                //    return LookupVariable(variableExpression.Name);
                default:
                    throw NonExhaustiveMatchException.For(value);
            }
        }
    }
}
