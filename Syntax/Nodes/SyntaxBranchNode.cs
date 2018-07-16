using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tree
{
    /// For lack of a better name, we use the tree terminology here of internal
    /// tree nodes being called "branches". Thus `SyntaxBranchNode` is by analogy
    /// to `SyntaxTree` an internal portion of the tree.
    ///
    /// Note: This allows the base class to be `SyntaxNode` since naming it
    ///     `Syntax` would conflict with the `Syntax` namespace
    public abstract class SyntaxBranchNode : SyntaxNode
    {
        public readonly IList<SyntaxNode> Children;

        protected SyntaxBranchNode(IEnumerable<SyntaxNode> children)
        {
            Children = children.ToList().AsReadOnly();
        }
    }
}
