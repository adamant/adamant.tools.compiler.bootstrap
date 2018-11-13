using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class CopyPlace : IValue
    {
        [NotNull] public readonly Place Place;

        public CopyPlace([NotNull] Place place)
        {
            Place = place;
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"copy {Place}";
        }
    }
}
