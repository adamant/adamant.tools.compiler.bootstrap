namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class Reference
    {
        public ObjectPlace Referent { get; }
        public Ownership Ownership { get; }
        public Access Access { get; }

        public Reference(ObjectPlace referent, Ownership ownership, Access access)
        {
            Referent = referent;
            Ownership = ownership;
            Access = access;
        }
    }
}
