using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class ParameterSyntax : SyntaxBranchNode
    {
        public ParameterSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
