namespace Adamant.Tools.Compiler.Bootstrap.Core.Promises
{
    public interface IPromise<out T>
    {
        bool IsFulfilled { get; }
        T Result { get; }
        /// <summary>
        /// Convert this promise to another kind of promise if possible.
        /// </summary>
        /// <remarks>
        /// This can't be an extension method because it would have to have two
        /// type parameters.
        /// </remarks>
        IPromise<TSub>? As<TSub>();
    }
}
