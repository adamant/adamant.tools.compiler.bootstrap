using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations
{
    /// <summary>
    /// An implicit conversion from `T` to `T?`
    /// </summary>
    internal class ImplicitOptionalConversionExpression : ImplicitConversionExpression, IImplicitOptionalConversionExpression
    {
        public OptionalType ConvertToType { get; }

        public ImplicitOptionalConversionExpression(
            IExpressionSyntax expression,
            OptionalType convertToType)
            : base(expression.Span, convertToType, expression, expression.Semantics.Assigned())
        {
            ConvertToType = convertToType;
        }

        public override string ToString()
        {
            return $"{Expression.ToGroupedString(OperatorPrecedence.Min)} ⟦as {ConvertToType}⟧";
        }
    }
}
