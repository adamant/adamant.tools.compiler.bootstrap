using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Literals;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Blocks;
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
                    {
                        var function =
                            (FunctionDeclarationAnalysis)declarations[functionScope.Syntax]
                                .AssertNotNull();
                        var variables = new Dictionary<string, IDeclarationAnalysis>();
                        foreach (var parameter in function.Parameters)
                            variables.Add(parameter.Name.Name.Text, parameter);

                        foreach (var declaration in function.Statements
                            .OfType<VariableDeclarationStatementAnalysis>())
                            variables.Add(declaration.Name.Name.Text, declaration);

                        functionScope.Bind(variables);

                        var blocks = new Dictionary<BlockSyntax, BlockAnalysis>();
                        GetAllBlocks(function, blocks);
                        foreach (var nestedScope in functionScope.NestedScopes.Cast<BlockScope>())
                            BindBlock(nestedScope, blocks);
                    }
                    break;
                case NamespaceScope namespaceScope:
                    {
                        // TODO bind correct names in the namespace
                        namespaceScope.Bind(new Dictionary<string, IDeclarationAnalysis>());
                        foreach (var nestedScope in namespaceScope.NestedScopes)
                            BindDeclaration(nestedScope);
                    }
                    break;
                case GenericsScope genericsScope:
                    {
                        var declaration = declarations[genericsScope.Syntax].AssertNotNull();
                        var parameters = new Dictionary<string, IDeclarationAnalysis>();
                        foreach (var parameter in declaration.GenericParameters)
                            parameters.Add(parameter.Name.Name.Text, parameter);

                        genericsScope.Bind(parameters);

                        foreach (var nestedScope in genericsScope.NestedScopes)
                            BindDeclaration(nestedScope);
                    }
                    break;
                default:
                    throw NonExhaustiveMatchException.For(scope);
            }
        }

        private void GetAllBlocks(
            [NotNull] FunctionDeclarationAnalysis function,
            [NotNull] Dictionary<BlockSyntax, BlockAnalysis> blocks)
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
            [NotNull] Dictionary<BlockSyntax, BlockAnalysis> blocks)
        {
            switch (expression)
            {
                case BlockAnalysis block:
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
                case ForeachExpressionAnalysis foreachExpression:
                    GetAllBlocks(foreachExpression.Block, blocks);
                    break;
                case InvocationAnalysis invocation:
                    foreach (var argument in invocation.Arguments)
                        GetAllBlocks(argument.Value, blocks);
                    break;
                case GenericInvocationAnalysis genericInvocation:
                    foreach (var argument in genericInvocation.Arguments)
                        GetAllBlocks(argument.Value, blocks);
                    break;
                case NewObjectExpressionAnalysis newObjectExpression:
                    foreach (var argument in newObjectExpression.Arguments)
                        GetAllBlocks(argument.Value, blocks);
                    break;
                case InitStructExpressionAnalysis initStructExpression:
                    foreach (var argument in initStructExpression.Arguments)
                        GetAllBlocks(argument.Value, blocks);
                    break;
                case UnsafeExpressionAnalysis unsafeExpression:
                    GetAllBlocks(unsafeExpression.Expression, blocks);
                    break;
                case RefTypeAnalysis refType:
                    GetAllBlocks(refType.ReferencedType, blocks);
                    break;
                case IfExpressionAnalysis ifExpression:
                    GetAllBlocks(ifExpression.Condition, blocks);
                    GetAllBlocks(ifExpression.ThenBlock, blocks);
                    if (ifExpression.ElseClause != null)
                        GetAllBlocks(ifExpression.ElseClause, blocks);
                    break;
                case ResultExpressionAnalysis resultExpression:
                    GetAllBlocks(resultExpression.Expression, blocks);
                    break;
                case IntegerLiteralExpressionAnalysis _:
                case IdentifierNameAnalysis _:
                case BooleanLiteralExpressionAnalysis _:
                    // Do nothing
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        private void BindBlock(
            [NotNull] BlockScope scope,
            [NotNull] Dictionary<BlockSyntax, BlockAnalysis> blocks)
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
