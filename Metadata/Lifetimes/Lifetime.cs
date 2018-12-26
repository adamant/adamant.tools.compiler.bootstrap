namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes
{
    public abstract class Lifetime
    {
        public abstract bool IsOwned { get; }
        public abstract override string ToString();
    }
}
