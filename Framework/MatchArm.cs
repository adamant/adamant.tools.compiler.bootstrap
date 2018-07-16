using System;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    internal struct MatchArm<T>
    {
        internal readonly Predicate<T> condition;
        internal readonly Action<T> action;

        internal MatchArm(Predicate<T> condition, Action<T> action)
        {
            this.condition = condition;
            this.action = action;
        }
    }

    internal struct MatchArm<T, R>
    {
        internal readonly Predicate<T> condition;
        internal readonly Func<T, R> action;

        internal MatchArm(Predicate<T> condition, Func<T, R> action)
        {
            this.condition = condition;
            this.action = action;
        }
    }
}
