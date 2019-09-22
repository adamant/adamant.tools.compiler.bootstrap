using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class SelfTypeSyntax : TypeSyntax
    {
        public SelfTypeSyntax(TextSpan span)
            : base(span)
        {
        }

        public override string ToString()
        {
            return "Self";
        }
    }
}
