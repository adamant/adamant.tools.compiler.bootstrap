using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public abstract class Name : RootName
    {
        [NotNull]
        public abstract SimpleName UnqualifiedName { get; }

        [NotNull]
        public static Name From(
            [NotNull] SimpleName firstPart,
            [NotNull] [ItemNotNull] params SimpleName[] parts)
        {
            Name name = firstPart;
            foreach (var part in parts)
                name = name.Qualify(part);
            return name;
        }

        [NotNull]
        public override Name Qualify([NotNull] SimpleName name)
        {
            Requires.NotNull(nameof(name), name);
            return new QualifiedName(this, name);
        }

        public abstract bool IsDirectlyIn([NotNull] Name name);
    }
}
