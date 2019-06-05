using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ImplicitImmutabilityConversionExpression : ImplicitConversionExpression
    {
        public ExpressionSyntax Expression { get; }

        public ImplicitImmutabilityConversionExpression(
            ExpressionSyntax expression,
            ObjectType convertToType)
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
