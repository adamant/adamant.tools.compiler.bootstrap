using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class SelfTypeSyntax : TypeSyntax, ISelfTypeSyntax
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
