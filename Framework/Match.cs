namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class Match
    {
        public static ValueMatcher<T> On<T>(T value)
        {
            return new ValueMatcher<T>(value);
        }
    }

    public static class MatchInto<R>
    {
        public static ValueMatcher<T, R> On<T>(T value)
        {
            return new ValueMatcher<T, R>(value);
        }
    }
}
