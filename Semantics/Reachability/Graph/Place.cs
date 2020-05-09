using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal abstract class Place
    {
        protected readonly List<Reference> references = new List<Reference>();
        public IReadOnlyList<Reference> References { get; }
        public IEnumerable<HeapPlace> PossibleReferents => references.Select(r => r.Referent).Distinct();

        protected Place()
        {
            References = references.AsReadOnly();
        }

        public void ClearReferences() => references.Clear();

        public void MoveFrom(RootPlace variable)
        {
            throw new NotImplementedException();
        }

        public void BorrowFrom(RootPlace variable)
        {
            foreach (var reference in variable.References) references.Add(reference.Borrow());
        }

        public void ShareFrom(RootPlace variable)
        {
            references.AddRange(variable.References.Select(r => r.Share()).DistinctBy(r => r.Referent));
        }

        public void IdentityFrom(RootPlace variable)
        {
            references.AddRange(variable.References.Select(r => r.Identify()).DistinctBy(r => r.Referent));
        }
    }
}
