using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitConversions
{
    internal class ImplicitImmutabilityConversionExpression : ImplicitConversionExpression
    {
        public ImplicitImmutabilityConversionExpression(
            IExpressionSyntax expression,
            UserObjectType convertToType)
            : base(expression.Span, convertToType, expression)
        {
        }

        public override string ToString()
        {
            return $"({Expression}) (as immutable)";
        }
    }
}
