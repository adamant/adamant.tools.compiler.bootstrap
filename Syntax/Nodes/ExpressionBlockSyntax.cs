using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public abstract class ExpressionBlockSyntax : ExpressionSyntax
    {
        protected ExpressionBlockSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
