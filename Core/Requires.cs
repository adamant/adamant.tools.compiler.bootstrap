using System;
using System.ComponentModel;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public static class Requires
    {
        internal static void Positive(string name, int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(name, value, "Must not be greater than or equal to zero");
        }

        public static void InString(string inString, string name, TextSpan value)
        {
            if (value.Start >= value.Length)
                throw new ArgumentOutOfRangeException(name, value, $"Start not in string of length {value.Length}");
            if (value.End > value.Length)
                throw new ArgumentOutOfRangeException(name, value, $"End not in string of length {value.Length}");
        }

        public static void ValidEnum<E>(string name, E value)
            where E : Enum
        {
            if (!Enum.IsDefined(typeof(E), value))
                throw new InvalidEnumArgumentException(name, Convert.ToInt32(value), typeof(E));
        }
    }
}
