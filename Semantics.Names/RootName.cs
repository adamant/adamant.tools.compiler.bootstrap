using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    /// <summary>
    /// A root name is a name one can use to qualify a simple name. It may be
    /// the global namespace.
    /// </summary>
    public abstract class RootName
    {
        /// <summary>
        /// Construct a new name by qualifying the given simple name with this
        /// one.
        /// </summary>
        [NotNull]
        public abstract Name Qualify([NotNull] SimpleName name);
    }
}
