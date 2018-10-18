using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals
{
    public abstract class LiteralExpressionSyntax : ExpressionSyntax
    {
        protected LiteralExpressionSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
