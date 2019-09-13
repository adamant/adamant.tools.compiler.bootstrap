using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(ResultExpressionSyntax),
        typeof(BlockSyntax))]
    public abstract class ExpressionBlockSyntax : ExpressionSyntax
    {
        protected ExpressionBlockSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
