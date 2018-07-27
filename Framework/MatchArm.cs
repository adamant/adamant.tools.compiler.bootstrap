using System;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    internal struct MatchArm<T>
    {
        internal readonly Predicate<T> Condition;
        internal readonly Action<T> Action;

        internal MatchArm(Predicate<T> condition, Action<T> action)
        {
            Condition = condition;
            Action = action;
        }
    }

    internal struct MatchArm<T, R>
    {
        internal readonly Predicate<T> Condition;
        internal readonly Func<T, R> Action;

        internal MatchArm(Predicate<T> condition, Func<T, R> action)
        {
            Condition = condition;
            Action = action;
        }
    }
}
