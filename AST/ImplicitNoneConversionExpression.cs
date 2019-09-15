using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// An implicit conversion from `none` of type `never?` to some other optional
    /// type. For example, a conversion to `int?`
    /// </summary>
    public class ImplicitNoneConversionExpression : ImplicitConversionExpression
    {
        public ExpressionSyntax Expression { get; }
        public OptionalType ConvertToType { get; }

        public ImplicitNoneConversionExpression(
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
