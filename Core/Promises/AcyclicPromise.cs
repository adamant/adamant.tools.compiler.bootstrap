using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Core.Promises
{
    /// <summary>
    /// A promise that helps in detecting cycles in the computation of promised
    /// values so that they can be forced to be acyclic.
    /// </summary>
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public class AcyclicPromise<T> : IPromise<T>
    {
        private PromiseState state;
        public bool IsFulfilled => state == PromiseState.Fulfilled;
        private T value = default!;

        public AcyclicPromise()
        {
            state = PromiseState.Pending;
        }

        public AcyclicPromise(T value)
        {
            this.value = value;
            state = PromiseState.Fulfilled;
        }

        [DebuggerHidden]
        public void BeginFulfilling()
        {
            Requires.That(nameof(state), state == PromiseState.Pending, "must be pending is " + state);
            state = PromiseState.InProgress;
        }

        [DebuggerHidden]
        public bool TryBeginFulfilling(Action? whenInProgress = null)
        {
            switch (state)
            {
                default:
                    throw ExhaustiveMatch.Failed(state);
                case PromiseState.InProgress:
                    whenInProgress?.Invoke();
                    return false;
                case PromiseState.Fulfilled:
                    // We have already resolved it
                    return false;
                case PromiseState.Pending:
                    state = PromiseState.InProgress;
                    // we need to compute it
                    return true;
            }
        }

        [DebuggerHidden]
        public T Fulfill(T value)
        {
            Requires.That(nameof(state), state == PromiseState.InProgress, "must be in progress is " + state);
            this.value = value;
            state = PromiseState.Fulfilled;
            return value;
        }

        [DebuggerHidden]
        public T Result
        {
            get
            {
                if (!IsFulfilled) throw new InvalidOperationException("Promise not fulfilled");

                return value;
            }
        }

        public IPromise<TSub>? As<TSub>()
        {
            if (this is IPromise<TSub> convertedPromise) return convertedPromise;
            if (IsFulfilled && Result is TSub convertedValue)
                return new Promise<TSub>(convertedValue);
            return null;
        }

        // Useful for debugging
        public override string ToString()
        {
            switch (state)
            {
                default:
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
                    throw ExhaustiveMatch.Failed(state);
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

    public static class AcyclicPromise
    {
        public static AcyclicPromise<T> ForValue<T>(T value)
        {
            return new AcyclicPromise<T>(value);
        }
    }
}
