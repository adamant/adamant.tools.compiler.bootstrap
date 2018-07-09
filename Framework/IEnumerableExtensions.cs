using System.Collections.Generic;

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
    }
}
