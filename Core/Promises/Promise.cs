using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Core.Promises
{
    /// <summary>
    /// A simple promise of a future value. The value can be set only once.
    /// </summary>
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public class Promise<T> : IPromise<T>
    {
        public bool IsFulfilled { get; private set; }
        private T value = default!;

        [DebuggerHidden]
        public Promise() { }

        [DebuggerHidden]
        public Promise(T value)
        {
            this.value = value;
            IsFulfilled = true;
        }

        [DebuggerHidden]
        public T Fulfill(T value)
        {
            Requires.That(nameof(IsFulfilled), !IsFulfilled, "must not already be fulfilled");
            this.value = value;
            IsFulfilled = true;
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
            return IsFulfilled ? value?.ToString() ?? "⧼null⧽" : "⧼pending⧽";
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
