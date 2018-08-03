using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    /// For lack of a better name, we use the tree terminology here of internal
    /// tree nodes being called "branches". Thus `SyntaxBranchNode` is by analogy
    /// to `SyntaxTree` an internal portion of the tree.
    ///
    /// Note: This allows the base class to be `SyntaxNode` since naming it
    ///     `Syntax` would conflict with the `Syntax` namespace
    public abstract class SyntaxBranchNode : SyntaxNode
    {
        public readonly IReadOnlyList<SyntaxNode> Children;

        protected SyntaxBranchNode(IEnumerable<SyntaxNode> children)
        {
            Children = children.ToList().AsReadOnly();
        }

        /// No guarantee of the order is made.
        public IEnumerable<SyntaxBranchNode> DescendantBranchesAndSelf()
        {
            var branches = new Stack<SyntaxBranchNode>();
            branches.Push(this);
            while (branches.Any())
            {
                var branch = branches.Pop();
                yield return branch;
                // If we reversed the children, we would have a pre-order traversal,
                // but this is more efficent
                foreach (var child in branch.Children.OfType<SyntaxBranchNode>())
                    branches.Push(child);
            }
        }
    }
}
