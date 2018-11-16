using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses.Builders
{
    public class ExpressionAnalysisBuilder : IExpressionAnalysisBuilder
    {
        [NotNull]
        private StatementAnalysisBuilder StatementBuilder
        {
            get
            {
                if (statementBuilder != null) return statementBuilder;

                statementBuilder = statementBuilderProvider.NotNull()();
                statementBuilderProvider = null;
                return statementBuilder.NotNull();
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

        [ContractAnnotation("expression:null => null; expression:notnull => notnull")]
        public ExpressionAnalysis BuildExpression(
            [NotNull] AnalysisContext context,
            [NotNull] Name functionName,
            [CanBeNull] ExpressionSyntax expression)
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
                case BinaryExpressionSyntax binaryOperatorExpression:
                    return new BinaryExpressionAnalysis(context, binaryOperatorExpression,
                        BuildExpression(context, functionName, binaryOperatorExpression.LeftOperand),
                        BuildExpression(context, functionName, binaryOperatorExpression.RightOperand));
                case UnaryExpressionSyntax unaryOperatorExpression:
                    var operand = BuildExpression(context, functionName, unaryOperatorExpression.Operand);
                    return new UnaryOperatorExpressionAnalysis(context, unaryOperatorExpression, operand);
                case IdentifierNameSyntax identifierName:
                    return new IdentifierNameAnalysis(context, identifierName);
                case GenericNameSyntax genericName:
                    return new GenericNameAnalysis(context, genericName,
                        genericName.Arguments.Select(a => BuildArgument(context, functionName, a)));
                case LifetimeTypeSyntax lifetimeType:
                    var typeName = BuildExpression(context, functionName, lifetimeType.Type);
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
                        BuildExpression(context, functionName, placementInitExpression.Initializer),
                        placementInitExpression.Arguments.Select(a => BuildArgument(context, functionName, a)));
                case LiteralExpressionSyntax literalExpression:
                    return BuildLiteralExpression(context, literalExpression.Literal);
                case ForeachExpressionSyntax foreachExpression:
                    // New context because a variable is declared
                    var loopBodyContext = context.InLocalVariableScope(foreachExpression);
                    return new ForeachExpressionAnalysis(context, foreachExpression,
                        functionName.Qualify((SimpleName)foreachExpression.Identifier.Value),
                        foreachExpression.Type != null ?
                            BuildExpression(context, functionName, foreachExpression.Type) : null,
                        BuildExpression(context, functionName, foreachExpression.InExpression),
                        BuildBlock(loopBodyContext, functionName, foreachExpression.Block));
                case WhileExpressionSyntax whileExpression:
                    return new WhileExpressionAnalysis(context, whileExpression,
                        BuildExpression(context, functionName, whileExpression.Condition),
                        BuildBlock(context, functionName, whileExpression.Block));
                case LoopExpressionSyntax loopExpression:
                    return new LoopExpressionAnalysis(context, loopExpression,
                        BuildBlock(context, functionName, loopExpression.Block));
                case BreakExpressionSyntax breakExpression:
                    return new BreakExpressionAnalysis(context, breakExpression,
                        breakExpression.Expression != null ? BuildExpression(context, functionName, breakExpression.Expression) : null);
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
                case MutableTypeSyntax mutableType:
                    return new MutableTypeAnalysis(context, mutableType, BuildExpression(context, functionName, mutableType.ReferencedType));
                case IfExpressionSyntax ifExpression:
                    return new IfExpressionAnalysis(context, ifExpression,
                        BuildExpression(context, functionName, ifExpression.Condition),
                        BuildExpressionBlock(context, functionName, ifExpression.ThenBlock),
                        BuildExpression(context, functionName, ifExpression.ElseClause));
                case ResultExpressionSyntax resultExpression:
                    return new ResultExpressionAnalysis(context, resultExpression,
                        BuildExpression(context, functionName, resultExpression.Expression));
                case MemberAccessExpressionSyntax memberAccessExpression:
                    return new MemberAccessExpressionAnalysis(context, memberAccessExpression,
                        BuildExpression(context, functionName, memberAccessExpression.Expression));
                case AssignmentExpressionSyntax assignmentExpression:
                {
                    return new AssignmentExpressionAnalysis(context, assignmentExpression,
                        BuildExpression(context, functionName, assignmentExpression.LeftOperand),
                        BuildExpression(context, functionName, assignmentExpression.RightOperand));
                }
                case null:
                    return null;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        [NotNull]
        private LiteralExpressionAnalysis BuildLiteralExpression(
            [NotNull] AnalysisContext context,
            [NotNull] ILiteralToken literal)
        {
            switch (literal)
            {
                case IBooleanLiteralToken booleanLiteral:
                    return new BooleanLiteralExpressionAnalysis(context, literal.Span, booleanLiteral.Value);
                case IIntegerLiteralToken integerLiteral:
                    return new IntegerLiteralExpressionAnalysis(context, literal.Span, integerLiteral.Value);
                case IStringLiteralToken stringLiteral:
                    return new StringLiteralExpressionAnalysis(context, literal.Span, stringLiteral.Value);
                case IUninitializedKeywordToken _:
                    return new UninitializedExpressionAnalysis(context, literal.Span);
                case INoneKeywordToken _:
                    throw new NotImplementedException();
                default:
                    throw NonExhaustiveMatchException.For(literal);
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
    }
}
