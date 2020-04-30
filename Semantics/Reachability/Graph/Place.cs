using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal abstract class Place
    {
        public PlaceIdentifier Identifier { get; }

        protected Place(PlaceIdentifier identifier)
        {
            Identifier = identifier;
        }

        public void Owns(Place place)
        {
            throw new System.NotImplementedException();
        }

        public void Shares(Place place)
        {
            throw new System.NotImplementedException();
        }

        public void PotentiallyOwns(Place place)
        {
            throw new System.NotImplementedException();
        }

        public void Borrows(Place place)
        {
            throw new System.NotImplementedException();
        }

        public void Identifies(Place place)
        {
            throw new System.NotImplementedException();
        }
    }
}
