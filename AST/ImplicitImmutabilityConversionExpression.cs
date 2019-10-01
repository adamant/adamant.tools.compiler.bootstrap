using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ImplicitImmutabilityConversionExpression : ImplicitConversionExpression
    {
        public IExpressionSyntax Expression { get; }

        public ImplicitImmutabilityConversionExpression(
            IExpressionSyntax expression,
            UserObjectType convertToType)
            : base(expression.Span, convertToType)
        {
            Expression = expression;
        }

        public override string ToString()
        {
            return $"({Expression}) (as immutable)";
        }
    }
}
