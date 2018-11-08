using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    /// <summary>
    /// A root name is a name one can build on. It may be the global namespace
    /// </summary>
    public abstract class RootName
    {
        [NotNull]
        public abstract Name Qualify([NotNull] SimpleName name);

        [NotNull]
        public Name Qualify([NotNull] string name)
        {
            return Qualify(new SimpleName(name));
        }
    }
}
