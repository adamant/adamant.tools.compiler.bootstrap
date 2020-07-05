using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;
using MoreLinq;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// A place in memory where a value can be stored
    /// </summary>
    public abstract class MemoryPlace
    {
        /// <summary>
        /// The graph this node is in
        /// </summary>
        public ReachabilityGraph Graph { get; }

        private readonly List<Reference> references = new List<Reference>();
        public IReadOnlyList<Reference> References { get; }
        public IEnumerable<HeapPlace> PossibleReferents => references.Select(r => r.Referent).Distinct();

        public bool IsAllocated { get; private set; } = true;

        protected MemoryPlace(ReachabilityGraph graph)
        {
            Graph = graph;
            References = references.AsReadOnly();
        }

        internal FixedList<Reference> StealReferences()
        {
            var stolenReferences = references.ToFixedList();
            references.Clear();
            Graph.Dirty();
            return stolenReferences;
        }

        // TODO verify that the place is part of the correct graph
        public void MoveFrom(StackPlace place)
        {
            references.AddRange(place.StealReferences());
            Graph.Dirty();
        }

        public void BorrowFrom(StackPlace place)
        {
            foreach (var reference in place.References)
                references.Add(reference.Borrow());
            Graph.Dirty();
        }

        public void ShareFrom(StackPlace place)
        {
            references.AddRange(place.References.Select(r => r.Share()).DistinctBy(r => r.Referent));
            Graph.Dirty();
        }

        public void IdentityFrom(StackPlace place)
        {
            references.AddRange(place.References.Select(r => r.Identify()).DistinctBy(r => r.Referent));
            Graph.Dirty();
        }

        public void AssignFrom(StackPlace place, ReferenceCapability referenceCapability)
        {
            switch (referenceCapability)
            {
                default:
                    throw ExhaustiveMatch.Failed(referenceCapability);
                case ReferenceCapability.Owned:
                case ReferenceCapability.OwnedMutable:
                case ReferenceCapability.Isolated:
                case ReferenceCapability.IsolatedMutable:
                case ReferenceCapability.Held:
                case ReferenceCapability.HeldMutable:
                    MoveFrom(place);
                    break;
                case ReferenceCapability.Shared:
                    ShareFrom(place);
                    break;
                case ReferenceCapability.Borrowed:
                    BorrowFrom(place);
                    break;
                case ReferenceCapability.Identity:
                    IdentityFrom(place);
                    break;
            }
        }

        internal virtual void Freed()
        {
            if (!IsAllocated)
                throw new Exception("Can't free memory twice");
            IsAllocated = false;
            ReleaseReferences();
        }

        private void ReleaseReferences()
        {
            foreach (var reference in references)
                if (reference.CouldHaveOwnership
                    || reference.Referent.GetCurrentAccess() is null) // not reachable
                    Graph.Delete(reference.Referent);
                else
                    Graph.LostReference(reference.Referent);

            foreach (var reference in references)
                reference.Release();
        }

        protected internal void AddReference(Reference reference)
        {
            references.Add(reference);
        }

        protected void AddReferences(IEnumerable<Reference> reference)
        {
            references.AddRange(reference);
        }

        internal void MarkReferencedObjects()
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
