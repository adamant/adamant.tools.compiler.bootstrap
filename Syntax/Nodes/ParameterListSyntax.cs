using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class ParameterListSyntax : SyntaxBranchNode
    {
        public ParameterListSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
