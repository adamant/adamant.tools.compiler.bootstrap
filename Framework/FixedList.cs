using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    // These attributes make it so FixedList<T> is displayed nicely in the debugger similar to List<T>
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    public class FixedList<T> : IReadOnlyList<T>
        where T : class
    {
        [NotNull] public static readonly FixedList<T> Empty = new FixedList<T>(Enumerable.Empty<T>());

        [NotNull, ItemNotNull] private readonly IReadOnlyList<T> items;

        [DebuggerStepThrough]
        public FixedList([NotNull, ItemNotNull] IEnumerable<T> items)
        {
            this.items = items.ItemsNotNull().ToList().AsReadOnly().NotNull();
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
