using System.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class Match
    {
        [DebuggerStepThrough]
        public static ValueMatcher<T> On<T>(T value)
        {
            return new ValueMatcher<T>(value);
        }
    }

    public static class MatchInto<R>
    {
        [DebuggerStepThrough]
        public static ValueMatcher<T, R> On<T>(T value)
        {
            return new ValueMatcher<T, R>(value);
        }
    }
}
