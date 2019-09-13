using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(SelfExpressionSyntax),
        typeof(BaseExpressionSyntax))]
    public abstract class InstanceExpressionSyntax : ExpressionSyntax
    {
        protected InstanceExpressionSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
