using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public abstract class MemberDeclarationSyntax : DeclarationSyntax
    {
        protected MemberDeclarationSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }

        protected MemberDeclarationSyntax(params SyntaxNode[] children)
            : base(children)
        {
        }
    }
}
