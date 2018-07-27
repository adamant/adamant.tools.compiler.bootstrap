using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Types
{
    public abstract class TypeSyntax : SyntaxBranchNode
    {
        protected TypeSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
