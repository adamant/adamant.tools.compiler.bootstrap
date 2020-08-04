using System.Collections.Generic;
using System.Diagnostics;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    /// <summary>
    /// A root name is a name one can use to qualify a simple name. It may be
    /// the global namespace.
    /// </summary>
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    [Closed(
        typeof(GlobalNamespaceName),
        typeof(MaybeQualifiedName))]
    public abstract class RootName
    {
        public abstract IEnumerable<SimpleName> Segments { get; }

        private protected RootName() { }

        /// <summary>
        /// Construct a new name by qualifying the given name with this one.
        /// </summary>
        public abstract MaybeQualifiedName Qualify(MaybeQualifiedName name);

        public MaybeQualifiedName Qualify(string name)
        {
            return Qualify(new SimpleName(name));
        }

        /// <summary>
        /// Gets all the names that this one is nested in. For example `Foo.Bar.Baz`
        /// would yield `Foo.Bar` and `Foo`.
        /// </summary>
        public abstract IEnumerable<MaybeQualifiedName> NestedInNames();

        /// <summary>
        /// Taking this name to be a namespace name, return it and all the names
        /// it is nested in.
        /// </summary>
        public abstract IEnumerable<MaybeQualifiedName> NamespaceNames();
    }
}
