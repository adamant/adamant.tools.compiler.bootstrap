using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public class FixedList<T> : IReadOnlyList<T>
        where T : class
    {
        [NotNull] public static readonly FixedList<T> Empty = new FixedList<T>(Enumerable.Empty<T>());

        [NotNull, ItemNotNull] private readonly IReadOnlyList<T> items;

        public FixedList([NotNull, ItemNotNull] IEnumerable<T> items)
        {
            Requires.NotNull(nameof(items), items);
            this.items = items.ItemsNotNull().ToReadOnlyList();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }

        public int Count => items.Count;

        public T this[int index] => items[index];
    }
}
