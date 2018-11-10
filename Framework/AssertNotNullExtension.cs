using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class AssertNotNullExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static T NotNull<T>([CanBeNull] this T value)
            where T : class
        {
            return value ?? throw new ArgumentNullException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ContractAnnotation("null => null; notnull => notnull"), ItemNotNull]
        public static IEnumerable<T> ItemsNotNull<T>([CanBeNull, ItemCanBeNull] this IEnumerable<T> values)
            where T : class
        {
            return values?.Select(value => value ?? throw new InvalidOperationException());
        }
    }
}
