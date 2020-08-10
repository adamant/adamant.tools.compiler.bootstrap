namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public interface IPromise<out T>
    {
        public T Result { get; }
        public IPromise<S>? As<S>();
    }
}
