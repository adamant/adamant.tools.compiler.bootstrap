using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    [DebuggerTypeProxy(typeof(DictionaryDebugView<,>))]
    public class FixedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        [NotNull] public static readonly FixedDictionary<TKey, TValue> Empty = new FixedDictionary<TKey, TValue>(new Dictionary<TKey, TValue>());

        [NotNull] private readonly IReadOnlyDictionary<TKey, TValue> items;

        [DebuggerStepThrough]
        public FixedDictionary([NotNull] IDictionary<TKey, TValue> items)
        {
            this.items = new ReadOnlyDictionary<TKey, TValue>(new Dictionary<TKey, TValue>(items));
        }

        [DebuggerStepThrough]
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        [DebuggerStepThrough]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }

        public int Count => items.Count;

        [DebuggerStepThrough]
        public bool ContainsKey(TKey key)
        {
            return items.ContainsKey(key);
        }

        [DebuggerStepThrough]
        public bool TryGetValue(TKey key, out TValue value)
        {
            return items.TryGetValue(key, out value);
        }

        public TValue this[TKey key] => items[key];

        public IEnumerable<TKey> Keys => items.Keys;

        public IEnumerable<TValue> Values => items.Values;
    }
}
