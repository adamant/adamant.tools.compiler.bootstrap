using System;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    internal class DerivedPromise<T, S> : IPromise<S>
    {
        private readonly IPromise<T> promise;
        private readonly Func<T, S> selector;

        public DerivedPromise(IPromise<T> promise, Func<T, S> selector)
        {
            this.promise = promise;
            this.selector = selector;
        }
        public PromiseState State => promise.State;

        public S Result => selector(promise.Result);

        public IPromise<TSub>? As<TSub>()
        {
            if (this is IPromise<TSub> convertedPromise) return convertedPromise;
            if (State == PromiseState.Fulfilled && Result is TSub convertedValue)
                return new Promise<TSub>(convertedValue);
            return null;
        }
    }
}
