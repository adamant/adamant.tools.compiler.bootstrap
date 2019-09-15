using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// An implicit conversion from `T` to `T?`
    /// </summary>
    public class ImplicitOptionalConversionExpression : ImplicitConversionExpression
    {
        public ExpressionSyntax Expression { get; }
        public OptionalType ConvertToType { get; }

        public ImplicitOptionalConversionExpression(
            ExpressionSyntax expression,
            OptionalType convertToType)
            : base(expression.Span, convertToType)
        {
            Expression = expression;
            ConvertToType = convertToType;
        }

        public override string ToString()
        {
            return $"({Expression}) (as) {ConvertToType}";
        }
    }
}
