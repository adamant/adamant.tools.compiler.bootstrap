using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ImplicitLiteralConversionExpression : ImplicitConversionExpression
    {
        public ExpressionSyntax Expression { get; }
        public DataType ConvertToType { get; }
        public ISymbol ConversionFunction { get; }

        public ImplicitLiteralConversionExpression(
            ExpressionSyntax expression,
            DataType convertToType,
            ISymbol conversionFunction)
            : base(expression.Span, convertToType)
        {
            Expression = expression;
            ConvertToType = convertToType;
            ConversionFunction = conversionFunction;
        }

        public override string ToString()
        {
            return $"({Expression}) (as) {ConvertToType}";
        }
    }
}
