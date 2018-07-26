using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public struct MatchBuilder<T>
    {
        private readonly IList<MatchArm<T>> arms;

        internal MatchBuilder(IList<MatchArm<T>> arms)
        {
            this.arms = arms;
        }

        [DebuggerStepThrough]
        public MatchBuilder<T> Is<S>(Action<S> arm)
            where S : T
        {
            arms.Add(new MatchArm<T>(v => v is S, v => arm((S)v)));
            return this;
        }

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
            arms.Add(new MatchArm<T>(v => v is S, Ignore));
            return this;
        }

        [DebuggerStepThrough]
        public void Any(Action<T> arm)
        {
            throw new NotImplementedException();
        }

        [DebuggerStepThrough]
        public void Any(Action arm)
        {
            throw new NotImplementedException();
        }

        private static void Ignore(T value)
        {
        }
    }

    public struct MatchBuilder<T, R>
    {
        private readonly IList<MatchArm<T, R>> arms;

        internal MatchBuilder(IList<MatchArm<T, R>> arms)
        {
            this.arms = arms;
        }

        [DebuggerStepThrough]
        public MatchBuilder<T, R> Is<S>(Func<S, R> arm)
            where S : T
        {
            arms.Add(new MatchArm<T, R>(v => v is S, v => arm((S)v)));
            return this;
        }
    }
}
