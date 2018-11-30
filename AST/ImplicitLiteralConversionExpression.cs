using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ImplicitLiteralConversionExpression : ImplicitConversionExpression
    {
        [NotNull] public ExpressionSyntax Expression { get; }
        [NotNull] public DataType ConvertToType { get; }
        [NotNull] public ISymbol ConversionFunction { get; }

        public ImplicitLiteralConversionExpression(
            [NotNull] ExpressionSyntax expression,
            [NotNull] DataType convertToType,
            [NotNull] ISymbol conversionFunction)
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
