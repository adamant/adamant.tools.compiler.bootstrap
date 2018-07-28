using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public struct MatchBuilder<T>
    {
        private readonly IList<MatchArm<T>> arms;

        [DebuggerStepThrough]
        internal MatchBuilder(IList<MatchArm<T>> arms)
        {
            this.arms = arms;
        }

        [DebuggerStepThrough]
        public MatchBuilder<T> Is<S>(Action<S> arm)
            where S : T
        {
            arms.Add(new MatchArm<T>(IsOfType<S>, MatchAction<T>.Create(arm)));
            return this;
        }

        [DebuggerStepThrough]
        internal static bool IsOfType<S>(T v) => v is S;

        //    public static void NotNull<T, S>(this IMatchStatementBuilder<T> builder, Action<S> arm)
        //        where S : T
        //        where T : class
        //    {
        //        throw new NotImplementedException();
        //    }

        [DebuggerStepThrough]
        public MatchBuilder<T> Ignore<S>()
            where S : T
        {
            arms.Add(new MatchArm<T>(IsOfType<S>, Ignore));
            return this;
        }

        [DebuggerStepThrough]
        public void Any(Action<T> arm)
        {
            arms.Add(new MatchArm<T>(arm));
        }

        [DebuggerStepThrough]
        public void Any(Action arm)
        {
            arms.Add(new MatchArm<T>(MatchAction<T>.Create(arm)));
        }

        [DebuggerStepThrough]
        private static void Ignore(T value)
        {
        }
    }

    public struct MatchBuilder<T, R>
    {
        private readonly IList<MatchArm<T, R>> arms;

        [DebuggerStepThrough]
        internal MatchBuilder(IList<MatchArm<T, R>> arms)
        {
            this.arms = arms;
        }

        [DebuggerStepThrough]
        public MatchBuilder<T, R> Is<S>(Func<S, R> arm)
            where S : T
        {
            arms.Add(new MatchArm<T, R>(MatchBuilder<T>.IsOfType<S>, MatchAction<T>.Create(arm)));
            return this;
        }

        [DebuggerStepThrough]
        public void Any(Func<T, R> arm)
        {
            arms.Add(new MatchArm<T, R>(arm));
        }

        [DebuggerStepThrough]
        public void Any(Func<R> arm)
        {
            arms.Add(new MatchArm<T, R>(MatchAction<T>.Create(arm)));
        }

    }
}
