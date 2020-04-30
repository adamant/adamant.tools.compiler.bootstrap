using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class ObjectPlace : Place
    {
        public new ObjectPlaceIdentifier Identifier { get; }

        public ObjectPlace(ObjectPlaceIdentifier identifier)
            : base(identifier)
        {
            Identifier = identifier;
        }
    }
}
