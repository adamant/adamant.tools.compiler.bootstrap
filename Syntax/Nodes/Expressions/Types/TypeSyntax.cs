using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types
{
    public abstract class TypeSyntax : ExpressionSyntax
    {
        protected TypeSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
