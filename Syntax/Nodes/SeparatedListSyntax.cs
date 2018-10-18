using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class SeparatedListSyntax<T> : SyntaxNode, IReadOnlyList<ISyntaxNodeOrToken>
        where T : SyntaxNode
    {
        [NotNull]
        public static readonly SeparatedListSyntax<T> Empty = new SeparatedListSyntax<T>(Enumerable.Empty<ISyntaxNodeOrToken>());

        [NotNull] [ItemNotNull] private readonly IReadOnlyList<ISyntaxNodeOrToken> children;

        public SeparatedListSyntax([NotNull][ItemNotNull] IEnumerable<ISyntaxNodeOrToken> children)
        {
            this.children = children.ToReadOnlyList();
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
                    Debug.Assert(item is SymbolToken, "Separator token missing or invalid in separated list.");
                }
            }
        }

        [NotNull]
        public IEnumerator<ISyntaxNodeOrToken> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        [NotNull]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => children.Count;

        [NotNull]
        public ISyntaxNodeOrToken this[int index] => children[index];

        [NotNull]
        public IEnumerable<T> Nodes()
        {
            return children.OfType<T>();
        }

        [NotNull]
        public IEnumerable<SymbolToken> Separators()
        {
            return children.OfType<SymbolToken>();
        }
    }
}
