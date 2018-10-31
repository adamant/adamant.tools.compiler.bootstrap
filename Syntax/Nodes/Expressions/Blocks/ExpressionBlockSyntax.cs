using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Blocks
{
    public abstract class ExpressionBlockSyntax : ExpressionSyntax
    {
        protected ExpressionBlockSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
