using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    /// <summary>
    /// An immutable list of syntax nodes. This allows code to accept a syntax
    /// list and use it directly, knowing it can't change. Use this rather
    /// than <see cref="ImmutableList{T}"/> because we don't need the ability
    /// to construct new lists from them which would introduce inefficiency and
    /// overhead.
    /// </summary>
    /// <typeparam name="TNode">Type of <see cref="SyntaxNode"/>s in the list</typeparam>
    public class SyntaxList<TNode> : IReadOnlyList<TNode>
        where TNode : SyntaxNode
    {
        public static readonly SyntaxList<TNode> Empty = new SyntaxList<TNode>(Enumerable.Empty<TNode>());

        private readonly List<TNode> nodes;

        public SyntaxList(IEnumerable<TNode> nodes)
        {
            this.nodes = nodes.ToList();
        }


        public IEnumerator<TNode> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)nodes).GetEnumerator();
        }

        public int Count => nodes.Count;

        public TNode this[int index] => nodes[index];
    }
}
