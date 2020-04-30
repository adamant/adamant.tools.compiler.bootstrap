namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability
{
    internal class CallerPlaceScope : PlaceScope
    {
        public CallerPlaceScope(PlaceList places)
            : base(places) { }

        public override CallerPlaceScope CallerScope => this;
    }
}
