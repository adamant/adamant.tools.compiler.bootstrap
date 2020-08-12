namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public interface IPromise<out T>
    {
        PromiseState State { get; }
        T Result { get; }
        IPromise<TSub>? As<TSub>();
    }
}
