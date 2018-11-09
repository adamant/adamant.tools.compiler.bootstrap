using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class SeparatedListSyntax<T> : NonTerminal, IReadOnlyList<ISyntaxNodeOrTokenPlace>
        where T : NonTerminal
    {
        [NotNull]
        public static readonly SeparatedListSyntax<T> Empty = new SeparatedListSyntax<T>(Enumerable.Empty<ISyntaxNodeOrTokenPlace>());

        [NotNull] [ItemNotNull] private readonly IReadOnlyList<ISyntaxNodeOrTokenPlace> children;

        public SeparatedListSyntax([NotNull][ItemNotNull] IEnumerable<ISyntaxNodeOrTokenPlace> children)
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
                    Debug.Assert(item is ITokenPlace, "Separator token missing or invalid in separated list.");
                }
            }
        }

        [NotNull]
        public IEnumerator<ISyntaxNodeOrTokenPlace> GetEnumerator()
        {
            return children.GetEnumerator().AssertNotNull();
        }

        [NotNull]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => children.Count;

        [NotNull]
        public ISyntaxNodeOrTokenPlace this[int index] => children[index];

        [NotNull]
        public IEnumerable<T> Nodes()
        {
            return children.OfType<T>();
        }

        [NotNull]
        public IEnumerable<ITokenPlace> Separators()
        {
            return children.OfType<ITokenPlace>();
        }
    }
}
