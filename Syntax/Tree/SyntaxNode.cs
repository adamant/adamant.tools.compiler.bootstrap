using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tree
{
    public abstract class SyntaxNode : Syntax
    {
        public readonly IList<Syntax> Children;

        protected SyntaxNode(IEnumerable<Syntax> children)
        {
            Children = children.ToList().AsReadOnly();
        }
    }
}
