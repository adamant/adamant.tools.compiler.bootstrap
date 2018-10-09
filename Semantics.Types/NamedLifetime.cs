using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class NamedLifetime : Lifetime
    {
        [NotNull] public readonly string Name;

        public NamedLifetime([NotNull] string name)
        {
            Requires.NotNull(nameof(name), name);
            Name = name;
        }

        public override bool IsOwned => false;

        [NotNull]
        public override string ToString()
        {
            return Name;
        }
    }
}
