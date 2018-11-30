using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    /// <summary>
    /// A root name is a name one can use to qualify a simple name. It may be
    /// the global namespace.
    /// </summary>
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public abstract class RootName
    {
        [NotNull, ItemNotNull] public abstract IEnumerable<SimpleName> Segments { get; }

        /// <summary>
        /// Construct a new name by qualifying the given name with this one.
        /// </summary>
        [NotNull]
        public abstract Name Qualify([NotNull] Name name);

        [NotNull]
        public Name Qualify([NotNull] string name)
        {
            return Qualify(new SimpleName(name));
        }
    }
}
