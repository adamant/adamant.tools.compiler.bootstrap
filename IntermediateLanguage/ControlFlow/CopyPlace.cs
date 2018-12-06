namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class CopyPlace : Operand
    {
        public readonly Place Place;

        public CopyPlace(Place place)
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
