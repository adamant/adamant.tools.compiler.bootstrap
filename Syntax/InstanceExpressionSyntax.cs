using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public abstract class InstanceExpressionSyntax : ExpressionSyntax
    {
        protected InstanceExpressionSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}