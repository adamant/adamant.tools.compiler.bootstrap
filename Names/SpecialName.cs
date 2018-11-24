using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    public static class SpecialName
    {
        [NotNull] public static readonly SimpleName Self = SimpleName.Special("self");
    }
}
