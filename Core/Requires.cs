using System;
using System.ComponentModel;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public static class Requires
    {
        public static void InString(string value, string startName, int start, string lengthName, int length)
        {
            if (start < 0 || start >= value.Length)
                throw new ArgumentOutOfRangeException(startName, start, $"Position not in string of length {value.Length}");
            if (length < 0)
                throw new ArgumentOutOfRangeException(lengthName, length, "Must be greater that zero");
            var end = start + length;
            if (end > value.Length)
                throw new ArgumentOutOfRangeException(lengthName, length, $"End not in string of length {value.Length} with start {start}");
        }

        public static void ValidEnum<E>(string name, E value)
            where E : Enum
        {
            if (!Enum.IsDefined(typeof(E), value))
                throw new InvalidEnumArgumentException(name, Convert.ToInt32(value), typeof(E));
        }
    }
}
