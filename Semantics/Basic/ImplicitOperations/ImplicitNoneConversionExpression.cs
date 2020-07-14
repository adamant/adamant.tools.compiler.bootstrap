using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations
{
    /// <summary>
    /// An implicit conversion from `none` of type `never?` to some other optional
    /// type. For example, a conversion to `int?`
    /// </summary>
    internal class ImplicitNoneConversionExpression : ImplicitConversionExpression, IImplicitNoneConversionExpression
    {
        public OptionalType ConvertToType { get; }

        public ImplicitNoneConversionExpression(
            IExpressionSyntax expression,
            OptionalType convertToType)
            // We can always copy the `none` literal
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
