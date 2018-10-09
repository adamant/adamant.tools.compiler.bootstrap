using System;
using System.ComponentModel;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    /// <summary>
    /// Note: The parameters for the parameter names are intentionally named
    /// `parameter` rather than `name` so that VS autocomplete won't try to
    /// complete to `name:` when you type `nameof...`
    /// </summary>
    public static class Requires
    {
        internal static void Positive([NotNull] string parameter, int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(parameter, value, "Must not be greater than or equal to zero");
        }

        public static void InString([NotNull] string inString, [NotNull] string parameter, TextSpan value)
        {
            // Start is allowed to be equal to length to allow for a zero length span after the last character
            if (value.Start > inString.Length)
                throw new ArgumentOutOfRangeException(parameter, value, $"Start not in string of length {inString.Length}");
            if (value.End > inString.Length)
                throw new ArgumentOutOfRangeException(parameter, value, $"End not in string of length {inString.Length}");
        }

        public static void InString([NotNull] string inString, [NotNull] string parameter, int value)
        {
            // Start is allowed to be equal to length to allow for a zero length span after the last character
            if (value < 0 || value >= inString.Length)
                throw new ArgumentOutOfRangeException(parameter, value, $"Value not in string of length {inString.Length}");
        }

        public static void NotNull([NotNull] string parameter, [NotNull] object value)
        {
            if (value == null)
                throw new ArgumentNullException(parameter);
        }

        public static void Null([NotNull] string parameter, [CanBeNull] object value)
        {
            if (value != null)
                throw new ArgumentException("Must be null", parameter);
        }

        public static void ValidEnum<E>([NotNull] string parameter, E value)
            where E : Enum
        {
            if (!Enum.IsDefined(typeof(E), value))
                throw new InvalidEnumArgumentException(parameter, Convert.ToInt32(value), typeof(E));
        }

        public static void That([NotNull] string parameter, bool condition)
        {
            if (!condition)
                throw new ArgumentException("Does not satisfy condition", parameter);
        }
    }
}
