using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Literals;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class ScopeBinder
    {
        [NotNull]
        private readonly Dictionary<MemberDeclarationSyntax, MemberDeclarationAnalysis>
            declarations;

        [NotNull] private readonly Dictionary<string, IDeclarationAnalysis> globalDeclarations;

        public ScopeBinder(
            [NotNull] [ItemNotNull] IEnumerable<MemberDeclarationAnalysis> declarationAnalyses)
        {
            this.declarations = declarationAnalyses.ToDictionary(a => a.Syntax, a => a);

            globalDeclarations = this.declarations.Values.AssertNotNull()
                .Where(IsGlobalDeclaration)
                .ToDictionary(d => d.Name.Name.Text, d => d as IDeclarationAnalysis);
        }

        private static bool IsGlobalDeclaration([NotNull] IDeclarationAnalysis declaration)
        {
            return !declaration.Name.Qualifier.Any();
        }

        public void Bind([NotNull] CompilationUnitScope scope)
        {
            Requires.NotNull(nameof(scope), scope);
            scope.Bind(globalDeclarations);
            foreach (var nestedScope in scope.NestedScopes)
                BindDeclaration(nestedScope);
        }

        private void BindDeclaration([NotNull] LexicalScope scope)
        {
            Requires.NotNull(nameof(scope), scope);
            switch (scope)
            {
                case FunctionScope functionScope:
                    var function =
                        (FunctionDeclarationAnalysis)declarations[functionScope.Syntax]
                            .AssertNotNull();
                    var variableDeclarations = new Dictionary<string, IDeclarationAnalysis>();
                    foreach (var parameter in function.Parameters)
                        variableDeclarations.Add(parameter.Name.Name.Text, parameter);
                    foreach (var declaration in function.Statements
                        .OfType<VariableDeclarationStatementAnalysis>())
                        variableDeclarations.Add(declaration.Name.Name.Text, declaration);

                    functionScope.Bind(variableDeclarations);

                    var blocks = new Dictionary<BlockExpressionSyntax, BlockExpressionAnalysis>();
                    GetAllBlocks(function, blocks);
                    foreach (var nestedScope in scope.NestedScopes.Cast<BlockScope>())
                        BindBlock(nestedScope, blocks);
                    break;
                default:
                    throw NonExhaustiveMatchException.For(scope);
            }
        }

        private void GetAllBlocks(
            [NotNull] FunctionDeclarationAnalysis function,
            [NotNull] Dictionary<BlockExpressionSyntax, BlockExpressionAnalysis> blocks)
        {
            foreach (var statement in function.Statements)
                switch (statement)
                {
                    case ExpressionStatementAnalysis expressionStatement:
                        GetAllBlocks(expressionStatement.Expression, blocks);
                        break;
                    case VariableDeclarationStatementAnalysis variableDeclaration:
                        if (variableDeclaration.Initializer != null)
                            GetAllBlocks(variableDeclaration.Initializer, blocks);
                        break;
                    default:
                        throw NonExhaustiveMatchException.For(statement);
                }
        }

        private void GetAllBlocks(
            [NotNull] ExpressionAnalysis expression,
            [NotNull] Dictionary<BlockExpressionSyntax, BlockExpressionAnalysis> blocks)
        {
            switch (expression)
            {
                case BlockExpressionAnalysis block:
                    blocks.Add(block.Syntax, block);
                    break;
                case ReturnExpressionAnalysis returnExpression:
                    if (returnExpression.ReturnExpression != null)
                        GetAllBlocks(returnExpression.ReturnExpression, blocks);
                    break;
                case BinaryOperatorExpressionAnalysis binaryOperatorExpression:
                    GetAllBlocks(binaryOperatorExpression.LeftOperand, blocks);
                    GetAllBlocks(binaryOperatorExpression.RightOperand, blocks);
                    break;
                case UnaryOperatorExpressionAnalysis unaryOperatorExpression:
                    GetAllBlocks(unaryOperatorExpression.Operand, blocks);
                    break;
                case IntegerLiteralExpressionAnalysis _:
                case IdentifierNameAnalysis _:
                    // Do nothing
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private void BindBlock(
            [NotNull] BlockScope scope,
            [NotNull] Dictionary<BlockExpressionSyntax, BlockExpressionAnalysis> blocks)
        {
            Requires.NotNull(nameof(scope), scope);
            Requires.NotNull(nameof(blocks), blocks);

            var block = blocks[scope.Syntax].AssertNotNull();
            var variableDeclarations = new Dictionary<string, IDeclarationAnalysis>();
            foreach (var declaration in block.Statements.OfType<VariableDeclarationStatementAnalysis>())
                variableDeclarations.Add(declaration.Name.Name.Text, declaration);

            foreach (var nestedScope in scope.NestedScopes.Cast<BlockScope>())
                BindBlock(nestedScope, blocks);

            scope.Bind(variableDeclarations);
        }
    }
}
