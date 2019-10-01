using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// This kind of conversion is inserted when there is an implicit conversion
    /// from a literal to some data type.
    /// </summary>
    public class ImplicitStringLiteralConversionExpression : ImplicitConversionExpression
    {
        public IStringLiteralExpressionSyntax Expression { get; }
        public DataType ConvertToType { get; }
        public ISymbol ConversionFunction { get; }

        public ImplicitStringLiteralConversionExpression(
            IStringLiteralExpressionSyntax expression,
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
