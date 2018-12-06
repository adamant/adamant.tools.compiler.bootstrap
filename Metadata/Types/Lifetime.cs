namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public abstract class Lifetime
    {
        public abstract bool IsOwned { get; }
        public abstract override string ToString();
    }
}
