using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    /// <summary>
    /// A name of a declaration. Note that names could even refer to local
    /// variables etc.
    /// </summary>
    public abstract class Name : RootName
    {
        /// <summary>
        /// The unqualified (i.e. SimpleName) portion of the name
        /// </summary>
        [NotNull]
        public abstract SimpleName UnqualifiedName { get; }

        /// <summary>
        /// Construct a name from its segments
        /// </summary>
        [NotNull]
        public static Name From(
            [NotNull] SimpleName firstSegment,
            [NotNull] [ItemNotNull] params SimpleName[] segments)
        {
            Requires.NotNull(nameof(firstSegment), firstSegment);
            Name name = firstSegment;
            foreach (var segment in segments)
                name = name.Qualify(segment);
            return name;
        }

        [NotNull]
        public override Name Qualify([NotNull] SimpleName name)
        {
            Requires.NotNull(nameof(name), name);
            return new QualifiedName(this, name);
        }

        public abstract bool HasQualifier([NotNull] Name name);
    }
}
