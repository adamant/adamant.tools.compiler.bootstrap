using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitConversions
{
    /// <summary>
    /// This kind of conversion is inserted when there is an implicit conversion
    /// from a literal to some data type.
    /// </summary>
    internal class ImplicitStringLiteralConversionExpression : ImplicitConversionExpression, IImplicitStringLiteralConversionExpression
    {
        public new IStringLiteralExpressionSyntax Expression => (IStringLiteralExpressionSyntax)base.Expression;
        public DataType ConvertToType { get; }
        public ISymbol ConversionFunction { get; }

        public ImplicitStringLiteralConversionExpression(
            IStringLiteralExpressionSyntax expression,
            DataType convertToType,
            ISymbol conversionFunction)
            : base(expression.Span, convertToType, expression)
        {
            ConvertToType = convertToType;
            ConversionFunction = conversionFunction;
        }

        public override string ToString()
        {
            return $"({Expression}) (as) {ConvertToType}";
        }
    }
}
