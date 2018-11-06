using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Builders
{
    public class ExpressionAnalysisBuilder : IExpressionAnalysisBuilder
    {
        [NotNull]
        private StatementAnalysisBuilder StatementBuilder
        {
            get
            {
                if (statementBuilder != null) return statementBuilder;

                statementBuilder = statementBuilderProvider.AssertNotNull()();
                statementBuilderProvider = null;
                return statementBuilder.AssertNotNull();
            }
        }
        [CanBeNull] private StatementAnalysisBuilder statementBuilder;
        [CanBeNull] private Func<StatementAnalysisBuilder> statementBuilderProvider;

        /// <summary>
        /// Because of a circular dependency, we don't take a <see cref="StatementAnalysisBuilder"/>
        /// but a function we can call later to get it.
        /// </summary>
        /// <param name="statementBuilderProvider"></param>
        public ExpressionAnalysisBuilder(
            [NotNull] Func<StatementAnalysisBuilder> statementBuilderProvider)
        {
            this.statementBuilderProvider = statementBuilderProvider;
        }

        [NotNull]
        public ExpressionAnalysis BuildExpression(
            [NotNull] AnalysisContext context,
            [NotNull] Name functionName,
            [NotNull] ExpressionSyntax expression)
        {
            switch (expression)
            {
                case ReturnExpressionSyntax returnExpression:
                    ExpressionAnalysis returnValue = null;
                    if (returnExpression.ReturnValue != null)
                        returnValue = BuildExpression(context, functionName, returnExpression.ReturnValue);
                    return new ReturnExpressionAnalysis(context, returnExpression, returnValue);
                case PrimitiveTypeSyntax primitiveType:
                    return new PrimitiveTypeAnalysis(context, primitiveType);
                case IntegerLiteralExpressionSyntax integerLiteral:
                    return new IntegerLiteralExpressionAnalysis(context, integerLiteral);
                case StringLiteralExpressionSyntax stringLiteral:
                    return new StringLiteralExpressionAnalysis(context, stringLiteral);
                case BinaryOperatorExpressionSyntax binaryOperatorExpression:
                    var leftOperand = BuildExpression(context, functionName, binaryOperatorExpression.LeftOperand);
                    var rightOperand = BuildExpression(context, functionName, binaryOperatorExpression.RightOperand);
                    return new BinaryOperatorExpressionAnalysis(context, binaryOperatorExpression, leftOperand, rightOperand);
                case UnaryOperatorExpressionSyntax unaryOperatorExpression:
                    var operand = BuildExpression(context, functionName, unaryOperatorExpression.Operand);
                    return new UnaryOperatorExpressionAnalysis(context, unaryOperatorExpression, operand);
                case IdentifierNameSyntax identifierName:
                    return new IdentifierNameAnalysis(context, identifierName);
                case GenericNameSyntax genericName:
                    return new GenericNameAnalysis(context, genericName,
                        genericName.Arguments.Select(a => BuildArgument(context, functionName, a)));
                case LifetimeTypeSyntax lifetimeType:
                    var typeName = BuildExpression(context, functionName, lifetimeType.TypeExpression);
                    return new LifetimeTypeAnalysis(context, lifetimeType, typeName);
                case BlockSyntax blockExpression:
                    return BuildBlock(context, functionName, blockExpression);
                case NewObjectExpressionSyntax newObjectExpression:
                    return new NewObjectExpressionAnalysis(context, newObjectExpression,
                        BuildExpression(context, functionName, newObjectExpression.Constructor),
                        newObjectExpression.Arguments.Select(a => BuildArgument(context, functionName, a)));
                case PlacementInitExpressionSyntax placementInitExpression:
                    return new PlacementInitExpressionAnalysis(context, placementInitExpression,
                        BuildExpression(context, functionName, placementInitExpression.PlaceExpression),
                        BuildExpression(context, functionName, placementInitExpression.Constructor),
                        placementInitExpression.Arguments.Select(a => BuildArgument(context, functionName, a)));
                case BooleanLiteralExpressionSyntax booleanLiteralExpression:
                    return new BooleanLiteralExpressionAnalysis(context, booleanLiteralExpression);
                case UninitializedExpressionSyntax uninitializedExpression:
                    return new UninitializedExpressionAnalysis(context, uninitializedExpression);
                case ForeachExpressionSyntax foreachExpression:
                    // New context because a variable is declared
                    var loopBodyContext = context.InLocalVariableScope(foreachExpression);
                    return new ForeachExpressionAnalysis(context, foreachExpression,
                        functionName.Qualify(foreachExpression.Identifier.Value ?? "_"),
                        foreachExpression.TypeExpression != null ?
                            BuildExpression(context, functionName, foreachExpression.TypeExpression) : null,
                        BuildExpression(context, functionName, foreachExpression.InExpression),
                        BuildBlock(loopBodyContext, functionName, foreachExpression.Block));
                case WhileExpressionSyntax whileExpression:
                    return new WhileExpressionAnalysis(context, whileExpression,
                        BuildExpression(context, functionName, whileExpression.Condition),
                        BuildBlock(context, functionName, whileExpression.Block));
                case InvocationSyntax invocation:
                    return new InvocationAnalysis(context, invocation,
                        BuildExpression(context, functionName, invocation.Callee),
                        invocation.Arguments.Select(a => BuildArgument(context, functionName, a)));
                case GenericsInvocationSyntax genericsInvocation:
                    return new GenericInvocationAnalysis(context, genericsInvocation,
                        BuildExpression(context, functionName, genericsInvocation.Callee),
                        genericsInvocation.Arguments.Select(a => BuildArgument(context, functionName, a)));
                case RefTypeSyntax refType:
                    return new RefTypeAnalysis(context, refType, BuildExpression(context, functionName, refType.ReferencedType));
                case UnsafeExpressionSyntax unsafeExpression:
                    return new UnsafeExpressionAnalysis(context, unsafeExpression,
                        BuildExpression(context, functionName, unsafeExpression.Expression));
                case ParenthesizedExpressionSyntax parenthesizedExpression:
                    return BuildExpression(context, functionName, parenthesizedExpression.Expression);
                case MutableTypeSyntax mutableType:
                    return new MutableTypeAnalysis(context, mutableType, BuildExpression(context, functionName, mutableType.ReferencedType));
                case IfExpressionSyntax ifExpression:
                    return new IfExpressionAnalysis(context, ifExpression,
                        BuildExpression(context, functionName, ifExpression.Condition),
                        BuildExpressionBlock(context, functionName, ifExpression.ThenBlock),
                        BuildElseClause(context, functionName, ifExpression.ElseClause));
                case ResultExpressionSyntax resultExpression:
                    return new ResultExpressionAnalysis(context, resultExpression,
                        BuildExpression(context, functionName, resultExpression.Expression));
                case MemberAccessExpressionSyntax memberAccessExpression:
                    return new MemberAccessExpressionAnalysis(context, memberAccessExpression,
                        BuildExpression(context, functionName, memberAccessExpression.Expression));
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        [NotNull]
        private ExpressionAnalysis BuildExpressionBlock(
            [NotNull] AnalysisContext context,
            [NotNull] Name functionName,
            [NotNull] ExpressionBlockSyntax expressionBlock)
        {
            switch (expressionBlock)
            {
                case BlockSyntax block:
                    return BuildBlock(context, functionName, block);
                case ResultExpressionSyntax resultExpression:
                    return BuildExpression(context, functionName, resultExpression);
                default:
                    throw NonExhaustiveMatchException.For(expressionBlock);
            }
        }

        [NotNull]
        private BlockAnalysis BuildBlock(
            [NotNull] AnalysisContext context,
            [NotNull] Name functionName,
            [NotNull] BlockSyntax block)
        {
            var blockContext = context.InLocalVariableScope(block);
            return new BlockAnalysis(context, block,
                block.Statements.Select(
                    s => StatementBuilder.Build(blockContext, functionName, s)));
        }

        [NotNull]
        public ArgumentAnalysis BuildArgument(
            [NotNull] AnalysisContext context,
            [NotNull] Name functionName,
            [NotNull] ArgumentSyntax argument)
        {
            return new ArgumentAnalysis(context, argument, BuildExpression(context, functionName, argument.Value));
        }

        [CanBeNull]
        private ExpressionAnalysis BuildElseClause(
            [NotNull] AnalysisContext context,
            [NotNull] Name functionName,
            [CanBeNull] ElseClauseSyntax elseClause)
        {
            if (elseClause == null) return null;
            return BuildExpression(context, functionName, elseClause.Expression);
        }
    }
}
