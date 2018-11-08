using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class LifetimeType : KnownType
    {
        [NotNull] public KnownType Type { get; }
        [NotNull] public Lifetime Lifetime { get; }

        public LifetimeType([NotNull] KnownType type, [NotNull] Lifetime lifetime)
        {
            Requires.NotNull(nameof(type), type);
            Requires.NotNull(nameof(lifetime), lifetime);
            Type = type;
            Lifetime = lifetime;
        }

        public bool IsOwned => Lifetime.IsOwned;

        [NotNull]
        public override string ToString()
        {
            return $"{Type}${Lifetime}";
        }
    }
}
