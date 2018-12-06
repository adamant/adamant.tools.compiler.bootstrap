using System.Collections.Generic;
using System.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    /// <summary>
    /// A root name is a name one can use to qualify a simple name. It may be
    /// the global namespace.
    /// </summary>
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public abstract class RootName
    {
        public abstract IEnumerable<SimpleName> Segments { get; }

        /// <summary>
        /// Construct a new name by qualifying the given name with this one.
        /// </summary>

        public abstract Name Qualify(Name name);

        public Name Qualify(string name)
        {
            return Qualify(new SimpleName(name));
        }
    }
}
