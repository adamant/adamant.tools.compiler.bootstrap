using System;
using System.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    /// A rather convoluted class needed because we can't apply the DebuggerStepThrough
    /// attribute to lambdas or local functions. So we need to be able to create
    /// our own closure like adapters.
    internal static class MatchAction<T>
    {
        [DebuggerStepThrough]
        public static Action<T> Create(Action action)
        {
            return new Adapter(action).Action;
        }

        internal class Adapter
        {
            private readonly Action action;

            [DebuggerStepThrough]
            internal Adapter(Action action)
            {
                this.action = action;
            }

            [DebuggerStepThrough]
            internal void Action(T value)
            {
                action();
            }
        }

        [DebuggerStepThrough]
        public static Action<T> Create<S>(Action<S> action)
            where S : T
        {
            return new Adapter<S>(action).Action;
        }

        internal class Adapter<S>
            where S : T
        {
            private readonly Action<S> action;

            [DebuggerStepThrough]
            public Adapter(Action<S> action)
            {
                this.action = action;
            }

            [DebuggerStepThrough]
            internal void Action(T value)
            {
                action((S)value);
            }
        }

        [DebuggerStepThrough]
        public static Func<T, R> Create<R>(Func<R> action)
        {
            return new FuncAdapter<R>(action).Action;
        }

        internal class FuncAdapter<R>
        {
            private readonly Func<R> action;

            [DebuggerStepThrough]
            internal FuncAdapter(Func<R> action)
            {
                this.action = action;
            }

            [DebuggerStepThrough]
            internal R Action(T value)
            {
                return action();
            }
        }

        [DebuggerStepThrough]
        public static Func<T, R> Create<S, R>(Func<S, R> action)
            where S : T
        {
            return new FuncAdapter<S, R>(action).ActionForAny;
        }

        internal class FuncAdapter<S, R>
            where S : T
        {
            internal readonly Func<S, R> Action;

            [DebuggerStepThrough]
            internal FuncAdapter(Func<S, R> action)
            {
                Action = action;
            }

            [DebuggerStepThrough]
            internal R ActionForAny(T value)
            {
                return Action((S)value);
            }
        }
    }
}
