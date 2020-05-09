using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

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
        public static IEnumerable<T> YieldValue<T>(this T? value)
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
        public static IEnumerable<(TFirst, TSecond)> CrossJoin<TFirst, TSecond>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second)
        {
            return first.SelectMany(_ => second, (f, s) => (f, s));
        }

        [DebuggerStepThrough]
        public static IEnumerable<(T Value, int Index)> Enumerate<T>(this IEnumerable<T> source)
        {
            return source.Select((v, i) => (v, i));
        }

        [DebuggerStepThrough]
        public static Queue<T> ToQueue<T>(this IEnumerable<T> source)
        {
            return new Queue<T>(source);
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> SelectMany<T>(this IEnumerable<IEnumerable<T>> source)
        {
            return source.SelectMany(items => items);
        }

        /// <summary>
        /// Performs an implicit cast. This is useful when C# is having trouble getting the correct type.
        /// </summary>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> SafeCast<T>(this IEnumerable<T> source)
        {
            // When the source implements multiple IEnumerable<T> and the next
            // Linq function takes IEnumerable (not generic) this shim
            // is needed to force the call to the correct GetEnumerator().
            // A Linq function that takes IEnumerable is OfType<T>()
            return new ImplicitCastEnumerable<T>(source);
        }

        private class ImplicitCastEnumerable<T> : IEnumerable<T>
        {
            private readonly IEnumerable<T> source;

            public ImplicitCastEnumerable(IEnumerable<T> source)
            {
                this.source = source;
            }

            public IEnumerator<T> GetEnumerator()
            {
                return source.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return source.GetEnumerator();
            }
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AnyTrue(this IEnumerable<bool> values)
        {
            return values.Any(v => v);
        }
    }
}
