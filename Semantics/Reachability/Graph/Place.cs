using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using ExhaustiveMatching;
using MoreLinq;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal abstract class Place
    {
        private readonly List<Reference> references = new List<Reference>();
        public IReadOnlyList<Reference> References { get; }
        public IEnumerable<HeapPlace> PossibleReferents => references.Select(r => r.Referent).Distinct();

        public bool IsAllocated { get; private set; } = true;

        protected Place()
        {
            References = references.AsReadOnly();
        }

        public FixedList<Reference> StealReferences()
        {
            var stolenReferences = references.ToFixedList();
            references.Clear();
            return stolenReferences;
        }

        public void MoveFrom(RootPlace place)
        {
            references.AddRange(place.StealReferences());
        }

        public void BorrowFrom(RootPlace place)
        {
            foreach (var reference in place.References)
                references.Add(reference.Borrow());
        }

        public void ShareFrom(RootPlace place)
        {
            references.AddRange(place.References.Select(r => r.Share()).DistinctBy(r => r.Referent));
        }

        public void IdentityFrom(RootPlace place)
        {
            references.AddRange(place.References.Select(r => r.Identify()).DistinctBy(r => r.Referent));
        }

        public virtual void Free()
        {
            if (!IsAllocated)
                throw new Exception("Can't free memory twice");
            IsAllocated = false;
            ReleaseReferences();
        }

        private void ReleaseReferences()
        {
            foreach (var reference in references.Where(r => r.CouldHaveOwnership))
                reference.Referent.Free();

            foreach (var reference in references)
                reference.Release();
        }

        protected void AddReference(Reference reference)
        {
            references.Add(reference);
        }

        protected void AddReferences(IEnumerable<Reference> reference)
        {
            references.AddRange(reference);
        }

        public void MarkReferencedObjects()
        {
            foreach (var reference in References)
            {
                var effectiveAccess = reference.EffectiveAccess();
                var referent = reference.Referent;
                switch (effectiveAccess)
                {
                    default:
                        throw ExhaustiveMatch.Failed(effectiveAccess);
                    case Access.ReadOnly:
                        if (reference.IsUsed)
                            referent.MarkReadOnly();
                        else
                            // If not used, we still need to recurse into it. We can identify it still.
                            referent.MarkIdentifiable();
                        break;
                    case Access.Identify:
                        referent.MarkIdentifiable();
                        break;
                    case Access.Mutable:
                        referent.MarkMutable();
                        break;
                }
            }
        }
    }
}
