using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public class FixedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        [NotNull] private readonly IReadOnlyDictionary<TKey, TValue> items;

        public FixedDictionary([NotNull] IDictionary<TKey, TValue> items)
        {
            this.items = new ReadOnlyDictionary<TKey, TValue>(new Dictionary<TKey, TValue>(items));
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }

        public int Count => items.Count;

        public bool ContainsKey(TKey key)
        {
            return items.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return items.TryGetValue(key, out value);
        }

        public TValue this[TKey key] => items[key];

        public IEnumerable<TKey> Keys => items.Keys;

        public IEnumerable<TValue> Values => items.Values;
    }
}
