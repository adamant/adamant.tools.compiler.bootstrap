using System;
using System.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    [Obsolete("Use C# pattern matching instead")]
    public static class Match
    {
        [DebuggerStepThrough]
        public static ValueMatcher<T> On<T>(T value)
        {
            return new ValueMatcher<T>(value);
        }
    }

    [Obsolete("Use C# pattern matching instead")]
    public static class MatchInto<R>
    {
        [DebuggerStepThrough]
        public static ValueMatcher<T, R> On<T>(T value)
        {
            return new ValueMatcher<T, R>(value);
        }
    }
}
