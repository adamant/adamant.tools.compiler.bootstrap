using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class EnumerableExtensions
    {
        [DebuggerStepThrough]
        public static IEnumerable<T> Yield<T>(this T value)
        {
            yield return value;
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> YieldValue<T>(this T value)
            where T : class
        {
            if (value != null)
                yield return value;
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> YieldValue<T>(this T? value)
            where T : struct
        {
            if (value != null)
                yield return value.Value;
        }

        [DebuggerStepThrough]
        public static FixedList<T> ToFixedList<T>(this IEnumerable<T> values)
            where T : class
        {
            return new FixedList<T>(values);
        }

        [DebuggerStepThrough]
        public static IEnumerable<TResult> CrossJoin<TFirst, TSecond, TResult>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            return first.SelectMany(_ => second, resultSelector);
        }

        [DebuggerStepThrough]
        public static IEnumerable<Tuple<TFirst, TSecond>> CrossJoin<TFirst, TSecond>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second)
        {
            return first.SelectMany(_ => second, Tuple.Create);
        }

        [DebuggerStepThrough]
        public static IEnumerable<(T, int)> Enumerate<T>(this IEnumerable<T> source)
        {
            return source.Select((v, i) => (v, i));
        }

        [DebuggerStepThrough]
        public static Queue<T> ToQueue<T>(this IEnumerable<T> source)
        {
            return new Queue<T>(source);
        }
    }
}
