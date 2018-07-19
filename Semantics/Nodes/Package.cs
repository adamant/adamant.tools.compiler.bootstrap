using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes
{
    public class Package : SemanticNode
    {
        public Package(IEnumerable<SemanticNode> children)
            : base(children)
        {
        }
    }
}
