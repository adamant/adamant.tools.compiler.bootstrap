using System;

namespace Adamant.Tools.Compiler.Bootstrap.Core.Promises
{
    public static class PromiseExtensions
    {
        public static IPromise<S> Select<T, S>(this IPromise<T> promise, Func<T, S> selector)
        {
            return new DerivedPromise<T, S>(promise, selector);
        }
    }
}
