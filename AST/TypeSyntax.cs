using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public abstract class TypeSyntax : ExpressionSyntax
    {
        protected TypeSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
