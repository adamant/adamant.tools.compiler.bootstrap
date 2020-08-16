using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations
{
    internal class ImplicitNumericConversionExpression : ImplicitConversionExpression, IImplicitNumericConversionExpressionSyntax
    {
        public NumericType ConvertToType { get; }

        public ImplicitNumericConversionExpression(
            IExpressionSyntax expression,
            NumericType convertToType)
            : base(expression.Span, convertToType, expression, ExpressionSemantics.Copy)
        {
            ConvertToType = convertToType;
        }

        public override string ToString()
        {
            return $"{Expression.ToGroupedString(OperatorPrecedence.Min)} ⟦as {ConvertToType}⟧";
        }
    }
}
