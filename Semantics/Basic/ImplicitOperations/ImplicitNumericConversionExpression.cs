using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitConversions
{
    internal class ImplicitNumericConversionExpression : ImplicitConversionExpression, IImplicitNumericConversionExpression
    {
        public NumericType ConvertToType { get; }

        public ImplicitNumericConversionExpression(
            IExpressionSyntax expression,
            NumericType convertToType)
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
