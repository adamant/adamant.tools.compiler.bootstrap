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
            // Start is allowed to be equal to length to allow for a zero length span after the last character
            if (value.Start > inString.Length)
                throw new ArgumentOutOfRangeException(name, value, $"Start not in string of length {inString.Length}");
            if (value.End > inString.Length)
                throw new ArgumentOutOfRangeException(name, value, $"End not in string of length {inString.Length}");
        }

        public static void ValidEnum<E>(string name, E value)
            where E : Enum
        {
            if (!Enum.IsDefined(typeof(E), value))
                throw new InvalidEnumArgumentException(name, Convert.ToInt32(value), typeof(E));
        }

        public static void That(string name, bool condition)
        {
            if (!condition)
                throw new ArgumentException("Does not satisfy condition", name);
        }
    }
}
