using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class LiteralExpressionSyntax : ExpressionSyntax
    {
        protected LiteralExpressionSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
