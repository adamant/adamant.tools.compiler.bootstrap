using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public class Promise<T> : IPromise<T>
    {
        public PromiseState State { get; private set; }
        private T value = default!;

        public Promise()
        {
            State = PromiseState.Pending;
        }

        public Promise(T value)
        {
            this.value = value;
            State = PromiseState.Fulfilled;
        }

        [DebuggerHidden]
        public void BeginFulfilling()
        {
            Requires.That(nameof(State), State == PromiseState.Pending, "must be pending is " + State);
            State = PromiseState.InProgress;
        }

        [DebuggerHidden]
        public bool TryBeginFulfilling(Action? whenInProgress = null)
        {
            switch (State)
            {
                default:
                    throw ExhaustiveMatch.Failed(State);
                case PromiseState.InProgress:
                    whenInProgress?.Invoke();
                    return false;
                case PromiseState.Fulfilled:
                    // We have already resolved it
                    return false;
                case PromiseState.Pending:
                    State = PromiseState.InProgress;
                    // we need to compute it
                    return true;
            }
        }

        [DebuggerHidden]
        public T Fulfill(T value)
        {
            Requires.That(nameof(State), State == PromiseState.InProgress, "must be in progress is " + State);
            State = PromiseState.Fulfilled;
            this.value = value ?? throw new ArgumentNullException(nameof(value));
            return value;
        }

        [DebuggerHidden]
        public T Result
        {
            get
            {
                if (State != PromiseState.Fulfilled) throw new InvalidOperationException("Promise not fulfilled");

                return value;
            }
        }

        public IPromise<S>? As<S>()
        {
            if (this is IPromise<S> promise) return promise;
            if (State == PromiseState.Fulfilled
                && value is S convertedValue)
                return new Promise<S>(convertedValue);
            return null;
        }

        // Useful for debugging
        public override string ToString()
        {
            switch (State)
            {
                default:
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
                    throw ExhaustiveMatch.Failed(State);
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
                case PromiseState.Pending:
                    return "⧼pending⧽";
                case PromiseState.InProgress:
                    return "⧼in progress⧽";
                case PromiseState.Fulfilled:
                    return value?.ToString() ?? "⧼null⧽";
            }
        }
    }

    public static class Promise
    {
        public static Promise<T> ForValue<T>(T value)
        {
            return new Promise<T>(value);
        }
    }
}
