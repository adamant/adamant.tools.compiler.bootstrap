using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(typeof(SelfExpressionSyntax))]
    public abstract class InstanceExpressionSyntax : ExpressionSyntax
    {
        protected InstanceExpressionSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
