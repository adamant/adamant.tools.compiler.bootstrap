using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers;
using static Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph.Access;
using static Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph.Ownership;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal abstract class Place
    {
        public PlaceIdentifier Identifier { get; }
        private readonly List<Reference> references = new List<Reference>();
        public IReadOnlyList<Reference> References => references.AsReadOnly();

        protected Place(PlaceIdentifier identifier)
        {
            Identifier = identifier;
        }

        protected void ClearReferences() => references.Clear();

        public void Owns(ObjectPlace @object, bool mutable)
        {
            references.Add(new Reference(@object, Ownership.Owns, mutable ? Mutable : ReadOnly));
        }

        public void PotentiallyOwns(ObjectPlace @object, bool b)
        {
            throw new System.NotImplementedException();
        }

        public void Borrows(ObjectPlace @object)
        {
            references.Add(new Reference(@object, None, Mutable));
        }

        public void Shares(ObjectPlace @object)
        {
            references.Add(new Reference(@object, None, ReadOnly));
        }

        public void Identifies(ObjectPlace @object)
        {
            references.Add(new Reference(@object, Ownership.None, Identity));
        }

        public void OwningIdentifies(ObjectPlace @object)
        {
            references.Add(new Reference(@object, Ownership.Owns, Identity));
        }
    }
}
