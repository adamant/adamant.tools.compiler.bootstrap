using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class DictionaryExtensions
    {
        [NotNull]
        public static FixedDictionary<TKey, TValue> ToFixedDictionary<TKey, TValue>(
            [NotNull] this IDictionary<TKey, TValue> dictionary)
        {
            return new FixedDictionary<TKey, TValue>(dictionary);
        }

        [NotNull]
        public static FixedDictionary<TKey, TValue> ToFixedDictionary<TSource, TKey, TValue>(
            [NotNull] this IEnumerable<TSource> source,
            [NotNull] Func<TSource, TKey> keySelector,
            [NotNull] Func<TSource, TValue> valueSelector)
        {
            return new FixedDictionary<TKey, TValue>(source.ToDictionary(keySelector, valueSelector));
        }

        [NotNull]
        public static FixedDictionary<TKey, TSource> ToFixedDictionary<TSource, TKey>(
            [NotNull] this IEnumerable<TSource> source,
            [NotNull] Func<TSource, TKey> keySelector)
        {
            return new FixedDictionary<TKey, TSource>(source.ToDictionary(keySelector));
        }
    }
}
