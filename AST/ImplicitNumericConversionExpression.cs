using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class ImplicitNumericConversionExpression : ImplicitConversionExpression
    {
        public ExpressionSyntax Expression { get; }
        public NumericType ConvertToType { get; }

        public ImplicitNumericConversionExpression(
            ExpressionSyntax expression,
            NumericType convertToType)
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
