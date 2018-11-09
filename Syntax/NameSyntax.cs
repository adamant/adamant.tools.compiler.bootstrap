using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public abstract class NameSyntax : TypeSyntax
    {
        protected NameSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
