using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    // These attributes make it so FixedList<T> is displayed nicely in the debugger similar to Set<T>
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    public class FixedSet<T> : IReadOnlyCollection<T>, IEquatable<FixedSet<T>>
    {
        public static readonly FixedSet<T> Empty = new FixedSet<T>(Enumerable.Empty<T>());

        // There is no IReadOnlySet interface, must use ISet
        private readonly ISet<T> items;

        [DebuggerStepThrough]
        public FixedSet(IEnumerable<T> items)
        {
            this.items = items.ToHashSet();
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

        #region Equality
        public override bool Equals(object? obj)
        {
            return Equals(obj as FixedSet<T>);
        }

        public bool Equals(FixedSet<T>? other)
        {
            return other != null && Count == other.Count && items.SetEquals(other.items);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            // Order the hash codes so there is a consistent hash order
            foreach (var item in items.OrderBy(i => i?.GetHashCode()))
                hash.Add(item);
            return hash.ToHashCode();
        }
        #endregion
    }
}
