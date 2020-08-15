using System;

namespace Adamant.Tools.Compiler.Bootstrap.Core.Promises
{
    /// <summary>
    /// A promise derived from another promise by transforming the value
    /// </summary>
    internal class DerivedPromise<T, S> : IPromise<S>
    {
        private readonly IPromise<T> promise;
        private readonly Func<T, S> selector;

        public DerivedPromise(IPromise<T> promise, Func<T, S> selector)
        {
            this.promise = promise;
            this.selector = selector;
        }

        public bool IsFulfilled => promise.IsFulfilled;

        public S Result => selector(promise.Result);

        public IPromise<TSub>? As<TSub>()
        {
            if (this is IPromise<TSub> convertedPromise) return convertedPromise;
            if (IsFulfilled && Result is TSub convertedValue)
                return new Promise<TSub>(convertedValue);
            return null;
        }
    }
}
