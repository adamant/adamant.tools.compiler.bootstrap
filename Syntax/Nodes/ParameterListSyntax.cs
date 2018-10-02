using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class ParameterListSyntax : SyntaxBranchNode
    {
        public IReadOnlyList<ParameterSyntax> Parameters { get; }

        public ParameterListSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            Parameters = Children.OfType<ParameterSyntax>().ToList().AsReadOnly();
        }
    }
}
