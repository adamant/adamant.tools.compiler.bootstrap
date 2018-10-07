using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class SeparatedListSyntax<T> : SyntaxNode, IReadOnlyList<ISyntaxNodeOrToken>
        where T : SyntaxNode
    {
        private readonly IReadOnlyList<ISyntaxNodeOrToken> children;

        public SeparatedListSyntax(IEnumerable<ISyntaxNodeOrToken> children)
        {
            this.children = children.ToList().AsReadOnly();
            Validate();
        }

        [Conditional("DEBUG")]
        private void Validate()
        {
            for (var i = 0; i < children.Count; i++)
            {
                var item = children[i];
                if ((i & 1) == 0)
                {
                    Debug.Assert(item is T, "Node missing in separated list.");
                }
                else
                {
                    Debug.Assert(item is Token t && t.Kind.HasValue() && t.Value == null, "Separator token missing or invalid in separated list.");
                }
            }
        }

        public IEnumerator<ISyntaxNodeOrToken> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => children.Count;

        public ISyntaxNodeOrToken this[int index] => children[index];

        public IEnumerable<T> Nodes()
        {
            return children.OfType<T>();
        }

        public IEnumerable<SimpleToken> Separators()
        {
            return children.OfType<Token>().Select(t => (SimpleToken)t);
        }
    }
}
