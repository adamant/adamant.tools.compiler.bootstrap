using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes
{
    [Closed(
        typeof(AnonymousLifetime),
        typeof(ForeverLifetime),
        typeof(NamedLifetime),
        typeof(NoLifetime),
        typeof(OwnedLifetime)//,
                             //typeof(RefLifetime)
        )]
    public abstract class Lifetime
    {
        public static Lifetime Owned = OwnedLifetime.Instance;
        public static Lifetime Forever = ForeverLifetime.Instance;
        public static Lifetime None = NoLifetime.Instance;

        public abstract bool IsOwned { get; }
        public abstract override string ToString();
    }
}
