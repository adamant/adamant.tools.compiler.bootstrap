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
                case LifetimeTypeSyntax lifetimeType:
                    var typeName = Build(context, functionName, lifetimeType.TypeName);
                    return new LifetimeTypeAnalysis(context, lifetimeType, typeName);
                case BlockExpressionSyntax blockExpression:
                    var blockContext = context.InBlock(blockExpression);
                    return new BlockExpressionAnalysis(context, blockExpression,
                        blockExpression.Statements.Select(s => StatementBuilder.Build(blockContext, functionName, s)));
                case NewObjectExpressionSyntax newObjectExpression:
                    return new NewObjectExpressionAnalysis(context, newObjectExpression,
                        Build(context, functionName, newObjectExpression.Constructor),
                        newObjectExpression.Arguments.Select(a => Build(context, functionName, a)));
                case BooleanLiteralExpressionSyntax booleanLiteralExpression:
                    return new BooleanLiteralExpressionAnalysis(context, booleanLiteralExpression);
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
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