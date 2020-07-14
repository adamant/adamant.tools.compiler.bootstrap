using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations
{
    internal class ImplicitNumericConversionExpression : ImplicitConversionExpression, IImplicitNumericConversionExpression
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
            return $"({Expression}) ⟦as {ConvertToType}⟧";
        }
    }
}
