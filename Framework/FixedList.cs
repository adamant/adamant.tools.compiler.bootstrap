using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    // These attributes make it so FixedList<T> is displayed nicely in the debugger similar to List<T>
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    public class FixedList<T> : IReadOnlyList<T>, IEquatable<FixedList<T>>
    {
        public static readonly FixedList<T> Empty = new FixedList<T>(Enumerable.Empty<T>());

        private readonly IReadOnlyList<T> items;

        [DebuggerStepThrough]
        public FixedList(IEnumerable<T> items)
        {
            this.items = items.ToList().AsReadOnly();
        }

        [DebuggerStepThrough]
        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        [DebuggerStepThrough]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }

        public int Count => items.Count;

        public T this[int index] => items[index];

        #region Equality
        public override bool Equals(object? obj)
        {
            return Equals(obj as FixedList<T>);
        }

        public bool Equals(FixedList<T>? other)
        {
            return other != null && Count == other.Count && items.SequenceEqual(other.items);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            foreach (var item in items) hash.Add(item);
            return hash.ToHashCode();
        }
        #endregion
    }
}
