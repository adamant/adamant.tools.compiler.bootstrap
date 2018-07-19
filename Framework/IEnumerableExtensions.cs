using System;
using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Yield<T>(this T value)
        {
            yield return value;
        }

        public static IEnumerable<T> YieldValue<T>(this T value)
            where T : class
        {
            if (value == null)
                yield break;
            else
                yield return value;
        }

        public static IEnumerable<T> YieldValue<T>(this T? value)
            where T : struct
        {
            if (value == null)
                yield break;
            else
                yield return value.Value;
        }

        public static IEnumerable<Tuple<TFirst, TSecond>> Zip<TFirst, TSecond>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second)
        {
            return first.Zip(second, (f, s) => Tuple.Create(f, s));
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static IEnumerable<TResult> CrossJoin<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            return first.SelectMany(_ => second, resultSelector);
        }

        public static IEnumerable<Tuple<TFirst, TSecond>> CrossJoin<TFirst, TSecond>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second)
        {
            return first.SelectMany(_ => second, (f, s) => Tuple.Create(f, s));
        }
    }
}
