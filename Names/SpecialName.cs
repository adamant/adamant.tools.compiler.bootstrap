using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    public static class SpecialName
    {
        [NotNull] public static readonly SimpleName Self = SimpleName.Special("self");
        [NotNull] public static readonly SimpleName Delete = SimpleName.Special("delete");
        [NotNull] public static readonly SimpleName Underscore = SimpleName.Special("_");
    }
}
