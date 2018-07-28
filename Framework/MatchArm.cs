using System;
using System.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    internal struct MatchArm<T>
    {
        internal readonly Predicate<T> Condition;
        internal readonly Action<T> Action;

        [DebuggerStepThrough]
        internal MatchArm(Predicate<T> condition, Action<T> action)
        {
            Condition = condition;
            Action = action;
        }

        [DebuggerStepThrough]
        internal MatchArm(Action<T> arm)
            : this(AlwaysMatches, arm)
        {
        }

        [DebuggerStepThrough]
        internal static bool AlwaysMatches(T value) => true;
    }

    internal struct MatchArm<T, R>
    {
        internal readonly Predicate<T> Condition;
        internal readonly Func<T, R> Action;

        [DebuggerStepThrough]
        internal MatchArm(Predicate<T> condition, Func<T, R> action)
        {
            Condition = condition;
            Action = action;
        }

        [DebuggerStepThrough]
        internal MatchArm(Func<T, R> arm)
            : this(MatchArm<T>.AlwaysMatches, arm)
        {
        }
    }
}
