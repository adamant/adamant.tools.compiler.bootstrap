using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitConversions
{
    /// <summary>
    /// An implicit conversion from `none` of type `never?` to some other optional
    /// type. For example, a conversion to `int?`
    /// </summary>
    internal class ImplicitNoneConversionExpression : ImplicitConversionExpression, IImplicitNoneConversionExpression
    {
        public OptionalType ConvertToType { get; }

        public ImplicitNoneConversionExpression(
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