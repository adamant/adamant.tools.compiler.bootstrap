using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations
{
    public abstract class DeclarationSyntax : SyntaxBranchNode
    {
        protected DeclarationSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
