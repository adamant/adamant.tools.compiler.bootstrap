using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class AssertNotNullExtension
    {
        /// <summary>
        /// Sometimes the system can't figure out that something isn't null.
        /// This let's us tell it that it is.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static T AssertNotNull<T>([CanBeNull] this T value)
            where T : class
        {
            return value ?? throw new InvalidOperationException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        [ItemNotNull]
        public static IEnumerable<T> AssertItemNotNull<T>([NotNull][ItemCanBeNull] this IEnumerable<T> values)
            where T : class
        {
            return values.Select(value => value ?? throw new InvalidOperationException());
        }
    }
}
