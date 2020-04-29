namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability
{
    public abstract class Place
    {
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
