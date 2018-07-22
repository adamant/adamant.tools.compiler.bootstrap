using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class ParameterListSyntax : SyntaxBranchNode
    {
        public IEnumerable<ParameterSyntax> Parameters => Children.OfType<ParameterSyntax>();

        public ParameterListSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
        }
    }
}
