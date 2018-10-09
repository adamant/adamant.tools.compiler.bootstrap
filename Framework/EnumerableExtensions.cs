using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class EnumerableExtensions
    {
        [NotNull]
        public static IEnumerable<T> Yield<T>(this T value)
        {
            yield return value;
        }

        [NotNull]
        [ItemNotNull]
        public static IEnumerable<T> YieldValue<T>(this T value)
            where T : class
        {
            if (value != null)
                yield return value;
        }

        [NotNull]
        public static IEnumerable<T> YieldValue<T>(this T? value)
            where T : struct
        {
            if (value != null)
                yield return value.Value;
        }

        [NotNull]
        public static IEnumerable<Tuple<TFirst, TSecond>> Zip<TFirst, TSecond>(
            [NotNull] this IEnumerable<TFirst> first,
            [NotNull] IEnumerable<TSecond> second)
        {
            return first.Zip(second, Tuple.Create);
        }

        [NotNull]
        public static IEnumerable<TResult> CrossJoin<TFirst, TSecond, TResult>(
            [NotNull] this IEnumerable<TFirst> first,
            [NotNull] IEnumerable<TSecond> second,
            [NotNull] Func<TFirst, TSecond, TResult> resultSelector)
        {
            return first.SelectMany(_ => second, resultSelector);
        }

        [NotNull]
        public static IEnumerable<Tuple<TFirst, TSecond>> CrossJoin<TFirst, TSecond>(
            [NotNull] this IEnumerable<TFirst> first,
            [NotNull] IEnumerable<TSecond> second)
        {
            return first.SelectMany(_ => second, Tuple.Create);
        }
    }
}
