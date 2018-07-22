using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes
{
    public class Package : SemanticNode
    {
        // Name
        // References
        // CompilationUnits
        // Symbol

        protected Package(IEnumerable<SemanticNode> children)
        {
        }

        protected override SyntaxNode GetSyntax()
        {
            throw new System.NotImplementedException();
        }
    }
}
