using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Literals;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class ExpressionAnalysisBuilder
    {
        public StatementAnalysisBuilder StatementBuilder { get; internal set; }

        [NotNull]
        public ExpressionAnalysis Build(
            [NotNull] AnalysisContext context,
            [NotNull] ExpressionSyntax expression)
        {
            switch (expression)
            {
                case ReturnExpressionSyntax returnExpression:
                    ExpressionAnalysis returnValue = null;
                    if (returnExpression.ReturnValue != null)
                        returnValue = Build(context, returnExpression.ReturnValue);
                    return new ReturnExpressionAnalysis(context, returnExpression, returnValue);
                case PrimitiveTypeSyntax primitiveType:
                    return new PrimitiveTypeAnalysis(context, primitiveType);
                case IntegerLiteralExpressionSyntax integerLiteral:
                    return new IntegerLiteralExpressionAnalysis(context, integerLiteral);
                case BinaryOperatorExpressionSyntax binaryOperatorExpression:
                    var leftOperand = Build(context, binaryOperatorExpression.LeftOperand);
                    var rightOperand = Build(context, binaryOperatorExpression.LeftOperand);
                    return new BinaryOperatorExpressionAnalysis(context, binaryOperatorExpression, leftOperand, rightOperand);
                case UnaryOperatorExpressionSyntax unaryOperatorExpression:
                    var operand = Build(context, unaryOperatorExpression.Operand);
                    return new UnaryOperatorExpressionAnalysis(context, unaryOperatorExpression, operand);
                case IdentifierNameSyntax identifierName:
                    return new IdentifierNameAnalysis(context, identifierName);
                case LifetimeTypeSyntax lifetimeType:
                    var typeName = Build(context, lifetimeType.TypeName);
                    return new LifetimeTypeAnalysis(context, lifetimeType, typeName);
                case BlockExpressionSyntax blockExpression:
                    return new BlockExpressionAnalysis(context, blockExpression,
                        blockExpression.Statements.Select(s => StatementBuilder.Build(context, s)));
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }


    }
}
