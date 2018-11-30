using System;
using System.ComponentModel;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    /// <summary>
    /// Note: The parameters for the parameter names are intentionally named
    /// `parameter` rather than `name` so that VS autocomplete won't try to
    /// complete to `name:` when you type `nameof...`
    /// </summary>
    public static class Requires
    {
        [DebuggerHidden]
        public static void Positive([NotNull] string parameter, int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(parameter, value, "Must be greater than or equal to zero");
        }

        [DebuggerHidden]
        public static void InString([NotNull] string inString, [NotNull] string parameter, int value)
        {
            // Start is allowed to be equal to length to allow for a zero length span after the last character
            if (value < 0 || value >= inString.Length)
                throw new ArgumentOutOfRangeException(parameter, value, $"Value not in string of length {inString.Length}");
        }

        [DebuggerHidden]
        public static void NotNull([NotNull] string parameter, [NotNull] object value)
        {
            if (value == null)
                throw new ArgumentNullException(parameter);
        }

        [DebuggerHidden]
        public static void Null([NotNull] string parameter, [CanBeNull] object value)
        {
            if (value != null)
                throw new ArgumentException("Must be null", parameter);
        }

        [DebuggerHidden]
        public static void ValidEnum<E>([NotNull] string parameter, E value)
            where E : Enum
        {
            if (!Enum.IsDefined(typeof(E), value))
                throw new InvalidEnumArgumentException(parameter, Convert.ToInt32(value), typeof(E));
        }

        [DebuggerHidden]
        public static void That([NotNull] string parameter, bool condition)
        {
            if (!condition)
                throw new ArgumentException("Does not satisfy condition", parameter);
        }
    }
}
