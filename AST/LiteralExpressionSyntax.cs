using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(NoneLiteralExpressionSyntax),
        typeof(UninitializedLiteralExpressionSyntax),
        typeof(StringLiteralExpressionSyntax),
        typeof(IntegerLiteralExpressionSyntax),
        typeof(BoolLiteralExpressionSyntax))]
    public abstract class LiteralExpressionSyntax : ExpressionSyntax
    {
        protected LiteralExpressionSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
