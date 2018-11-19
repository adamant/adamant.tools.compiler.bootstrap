using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class ExpressionBlockSyntax : ExpressionSyntax
    {
        protected ExpressionBlockSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
