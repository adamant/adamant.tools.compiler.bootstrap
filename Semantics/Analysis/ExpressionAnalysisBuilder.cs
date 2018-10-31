using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Literals;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Blocks;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
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
        public ExpressionAnalysis Build(
            [NotNull] AnalysisContext context,
            [NotNull] QualifiedName functionName,
            [NotNull] ExpressionSyntax expression)
        {
            switch (expression)
            {
                case ReturnExpressionSyntax returnExpression:
                    ExpressionAnalysis returnValue = null;
                    if (returnExpression.ReturnValue != null)
                        returnValue = Build(context, functionName, returnExpression.ReturnValue);
                    return new ReturnExpressionAnalysis(context, returnExpression, returnValue);
                case PrimitiveTypeSyntax primitiveType:
                    return new PrimitiveTypeAnalysis(context, primitiveType);
                case IntegerLiteralExpressionSyntax integerLiteral:
                    return new IntegerLiteralExpressionAnalysis(context, integerLiteral);
                case BinaryOperatorExpressionSyntax binaryOperatorExpression:
                    var leftOperand = Build(context, functionName, binaryOperatorExpression.LeftOperand);
                    var rightOperand = Build(context, functionName, binaryOperatorExpression.RightOperand);
                    return new BinaryOperatorExpressionAnalysis(context, binaryOperatorExpression, leftOperand, rightOperand);
                case UnaryOperatorExpressionSyntax unaryOperatorExpression:
                    var operand = Build(context, functionName, unaryOperatorExpression.Operand);
                    return new UnaryOperatorExpressionAnalysis(context, unaryOperatorExpression, operand);
                case IdentifierNameSyntax identifierName:
                    return new IdentifierNameAnalysis(context, identifierName);
                case GenericNameSyntax genericName:
                    return new GenericNameAnalysis(context, genericName,
                        genericName.Arguments.Select(a => Build(context, functionName, a)));
                case LifetimeTypeSyntax lifetimeType:
                    var typeName = Build(context, functionName, lifetimeType.TypeExpression);
                    return new LifetimeTypeAnalysis(context, lifetimeType, typeName);
                case BlockSyntax blockExpression:
                    return BuildBlock(context, functionName, blockExpression);
                case NewObjectExpressionSyntax newObjectExpression:
                    return new NewObjectExpressionAnalysis(context, newObjectExpression,
                        Build(context, functionName, newObjectExpression.Constructor),
                        newObjectExpression.Arguments.Select(a => Build(context, functionName, a)));
                case InitStructExpressionSyntax initStructExpression:
                    return new InitStructExpressionAnalysis(context, initStructExpression,
                        Build(context, functionName, initStructExpression.Constructor),
                        initStructExpression.Arguments.Select(a => Build(context, functionName, a)));
                case BooleanLiteralExpressionSyntax booleanLiteralExpression:
                    return new BooleanLiteralExpressionAnalysis(context, booleanLiteralExpression);
                case ForeachExpressionSyntax foreachExpression:
                    return new ForeachExpressionAnalysis(context, foreachExpression,
                        Build(context, functionName, foreachExpression.InExpression),
                        BuildBlock(context, functionName, foreachExpression.Block));
                case InvocationSyntax invocation:
                    return new InvocationAnalysis(context, invocation,
                        Build(context, functionName, invocation.Callee),
                        invocation.Arguments.Select(a => Build(context, functionName, a)));
                case GenericsInvocationSyntax genericsInvocation:
                    return new GenericInvocationAnalysis(context, genericsInvocation,
                        Build(context, functionName, genericsInvocation.Callee),
                        genericsInvocation.Arguments.Select(a => Build(context, functionName, a)));
                case RefTypeSyntax refType:
                    return new RefTypeAnalysis(context, refType, Build(context, functionName, refType.ReferencedType));
                case UnsafeExpressionSyntax unsafeExpression:
                    return new UnsafeExpressionAnalysis(context, unsafeExpression,
                        Build(context, functionName, unsafeExpression.Expression));
                case ParenthesizedExpressionSyntax parenthesizedExpression:
                    return Build(context, functionName, parenthesizedExpression.Expression);
                case MutableTypeSyntax mutableType:
                    return new MutableTypeAnalysis(context, mutableType, Build(context, functionName, mutableType.ReferencedType));
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        [NotNull]
        private BlockExpressionAnalysis BuildBlock(
            [NotNull] AnalysisContext context,
            [NotNull] QualifiedName functionName,
            [NotNull] BlockSyntax block)
        {
            var blockContext = context.InBlock(block);
            return new BlockExpressionAnalysis(context, block,
                block.Statements.Select(
                    s => StatementBuilder.Build(blockContext, functionName, s)));
        }

        [NotNull]
        public ArgumentAnalysis Build(
            [NotNull] AnalysisContext context,
            [NotNull] QualifiedName functionName,
            [NotNull] ArgumentSyntax argument)
        {
            return new ArgumentAnalysis(context, argument, Build(context, functionName, argument.Value));
        }
    }
}
