using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    public static class SpecialName
    {
        [NotNull] public static readonly SimpleName Self = SimpleName.Special("self");
        [NotNull] public static readonly SimpleName Base = SimpleName.Special("base");
        [NotNull] public static readonly SimpleName Ref = SimpleName.Special("ref");
        [NotNull] public static readonly SimpleName Owned = SimpleName.Special("owned");
        [NotNull] public static SimpleName Forever = SimpleName.Special("forever");
        [NotNull] public static readonly SimpleName Delete = SimpleName.Special("delete");
        [NotNull] public static readonly SimpleName Underscore = SimpleName.Special("_");
    }
}
