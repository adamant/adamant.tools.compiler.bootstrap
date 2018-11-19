using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public abstract class Lifetime
    {
        public abstract bool IsOwned { get; }
        [NotNull] public abstract override string ToString();
    }
}
