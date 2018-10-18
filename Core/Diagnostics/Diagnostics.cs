using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics
{
    /// <summary>
    /// An immutable list of diagnostics. This allows code to accept a diagnostic
    /// list and use it directly, knowing it can't change. Use this rather
    /// than <see cref="ImmutableList{T}"/> because we don't need the ability
    /// to construct new lists from them which would introduce inefficiency and
    /// overhead. This class also causes the null checking to work correctly
    /// </summary>
    public class Diagnostics : IReadOnlyList<Diagnostic>
    {
        [NotNull] public static readonly Diagnostics Empty = new Diagnostics(Enumerable.Empty<Diagnostic>());

        [NotNull] [ItemNotNull] private readonly List<Diagnostic> items;

        public Diagnostics([NotNull][ItemNotNull] IEnumerable<Diagnostic> items)
        {
            this.items = items.ToList();
            this.items.Sort();
        }

        // Internal constructor used by Diagnostics builder for performance
        internal Diagnostics([NotNull][ItemNotNull] List<Diagnostic> items)
        {
            this.items = items;
            this.items.Sort();
        }

        [NotNull]
        public IEnumerator<Diagnostic> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        [NotNull]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public int Count => items.Count;

        [NotNull]
        public Diagnostic this[int index] => items[index];
    }
}
