using System;
using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class DictionaryExtensions
    {
        public static FixedDictionary<TKey, TValue> ToFixedDictionary<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary)
            where TKey : notnull
        {
            return new FixedDictionary<TKey, TValue>(dictionary);
        }

        public static FixedDictionary<TKey, TValue> ToFixedDictionary<TSource, TKey, TValue>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TValue> valueSelector)
            where TKey : notnull
        {
            return new FixedDictionary<TKey, TValue>(source.ToDictionary(keySelector, valueSelector));
        }

        public static FixedDictionary<TKey, TValue> ToFixedDictionary<TKey, TValue>(
            this IEnumerable<(TKey Key, TValue Value)> source)
            where TKey : notnull
        {
            return new FixedDictionary<TKey, TValue>(source.ToDictionary(s => s.Key, s => s.Value));
        }

        public static FixedDictionary<TKey, TSource> ToFixedDictionary<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
            where TKey : notnull
        {
            return new FixedDictionary<TKey, TSource>(source.ToDictionary(keySelector));
        }
    }
}
