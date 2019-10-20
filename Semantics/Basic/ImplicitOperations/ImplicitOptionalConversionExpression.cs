using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations
{
    /// <summary>
    /// An implicit conversion from `T` to `T?`
    /// </summary>
    internal class ImplicitOptionalConversionExpression : ImplicitConversionExpression
    {
        public OptionalType ConvertToType { get; }

        public ImplicitOptionalConversionExpression(
            IExpressionSyntax expression,
            OptionalType convertToType)
            : base(expression.Span, convertToType, expression)
        {
            ConvertToType = convertToType;
        }

        public override string ToString()
        {
            return $"({Expression}) (as) {ConvertToType}";
        }
    }
}
