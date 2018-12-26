namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes
{
    public abstract class Lifetime
    {
        public static Lifetime Owned = OwnedLifetime.Instance;
        public static Lifetime Forever = ForeverLifetime.Instance;

        public abstract bool IsOwned { get; }
        public abstract override string ToString();
    }
}
