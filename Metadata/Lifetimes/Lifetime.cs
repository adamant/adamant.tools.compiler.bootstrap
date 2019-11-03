using System.Diagnostics.CodeAnalysis;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes
{
    [Closed(
        typeof(AnonymousLifetime),
        typeof(ForeverLifetime),
        typeof(NamedLifetime),
        typeof(NoLifetime),
        typeof(OwnedLifetime))]
    [SuppressMessage("Naming", "CA1724:Type names should not match namespaces", Justification = "Core to compiler domain")]
    public abstract class Lifetime
    {
        public static readonly Lifetime Owned = OwnedLifetime.Instance;
        public static readonly Lifetime Forever = ForeverLifetime.Instance;
        public static readonly Lifetime None = NoLifetime.Instance;

        public abstract bool IsOwned { get; }
        public abstract override string ToString();
    }
}
