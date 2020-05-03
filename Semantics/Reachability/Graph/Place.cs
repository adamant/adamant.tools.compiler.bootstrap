using System;
using System.Collections.Generic;
using System.Linq;
using static Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph.Access;
using static Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph.Ownership;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal abstract class Place
    {
        private readonly List<Reference> references = new List<Reference>();
        public IReadOnlyList<Reference> References { get; }
        public IEnumerable<HeapPlace> PossibleReferents => references.Select(r => r.Referent).Distinct();

        protected Place()
        {
            References = references.AsReadOnly();
        }

        protected void ClearReferences() => references.Clear();

        public void MoveFrom(Variable variable)
        {
            throw new NotImplementedException();
        }

        public void BorrowFrom(Variable variable)
        {
            foreach (var reference in variable.References)
                references.Add(reference.Borrow());
        }

        public void ShareFrom(Variable variable)
        {
            foreach (var heapPlace in variable.PossibleReferents)
                references.Add(heapPlace.NewSharedReference());
        }

        public void IdentityFrom(Variable variable)
        {
            foreach (var heapPlace in variable.PossibleReferents)
                references.Add(heapPlace.NewIdentityReference());
        }

        public void Owns(HeapPlace heapPlace, bool mutable)
        {
            references.Add(new Reference(heapPlace, Ownership.Owns, mutable ? Mutable : ReadOnly));
        }

        public void PotentiallyOwns(HeapPlace heapPlace, bool b)
        {
            throw new System.NotImplementedException();
        }

        public void Borrows(HeapPlace heapPlace)
        {
            references.Add(new Reference(heapPlace, None, Mutable));
        }

        public void Shares(HeapPlace heapPlace)
        {
            references.Add(new Reference(heapPlace, None, ReadOnly));
        }

        public void Identifies(HeapPlace heapPlace)
        {
            references.Add(new Reference(heapPlace, Ownership.None, Identity));
        }
    }
}
