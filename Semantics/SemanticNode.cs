using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticNode
    {
        public readonly IReadOnlyList<SemanticNode> Children;

        protected SemanticNode(IEnumerable<SemanticNode> children)
        {
            Children = children.ToList().AsReadOnly();
        }
    }
}
