using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ImplicitNumericConversionExpression : ImplicitConversionExpression
    {
        public IExpressionSyntax Expression { get; }
        public NumericType ConvertToType { get; }

        public ImplicitNumericConversionExpression(
            IExpressionSyntax expression,
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
