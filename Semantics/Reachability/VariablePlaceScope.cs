namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability
{
    /// <summary>
    /// A lexical scope of variable places used to track when variables come into
    /// and go out of scope.
    /// </summary>
    internal class VariablePlaceScope : PlaceScope
    {
        private readonly PlaceScope containingScope;

        public VariablePlaceScope(PlaceList places, PlaceScope containingScope)
            : base(places)
        {
            this.containingScope = containingScope;
        }

        public override CallerPlaceScope CallerScope => containingScope.CallerScope;
    }
}
