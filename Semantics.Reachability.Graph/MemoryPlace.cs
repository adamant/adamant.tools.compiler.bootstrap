using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Types;
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
        internal IReferenceGraph Graph { get; }

        private readonly List<IReference> references = new List<IReference>();
        public IReadOnlyList<IReference> References { get; }
        public IEnumerable<HeapPlace> PossibleReferents => references.Select(r => r.Referent).Distinct();

        public bool IsAllocated { get; private set; } = true;

        private protected MemoryPlace(IReferenceGraph graph)
        {
            Graph = graph;
            References = references.AsReadOnly();
        }

        internal FixedList<IReference> StealReferences()
        {
            var stolenReferences = references.ToFixedList();
            references.Clear();
            Graph.Dirty();
            return stolenReferences;
        }

        public void MoveFrom(StackPlace place)
        {
            if (place.Graph != Graph)
                throw new ArgumentException("Must be part of the same graph", nameof(place));

            references.AddRange(place.StealReferences());
            Graph.Dirty();
        }

        public void BorrowFrom(StackPlace place)
        {
            if (place.Graph != Graph)
                throw new ArgumentException("Must be part of the same graph", nameof(place));

            foreach (var reference in place.References)
                references.Add(reference.Borrow());
            Graph.Dirty();
        }

        public void ShareFrom(StackPlace place)
        {
            if (place.Graph != Graph)
                throw new ArgumentException("Must be part of the same graph", nameof(place));

            references.AddRange(place.References.Select(r => r.Share()).DistinctBy(r => r.Referent));
            Graph.Dirty();
        }

        public void IdentityFrom(StackPlace place)
        {
            if (place.Graph != Graph)
                throw new ArgumentException("Must be part of the same graph", nameof(place));

            references.AddRange(place.References.Select(r => r.Identify()).DistinctBy(r => r.Referent));
            Graph.Dirty();
        }

        public void AssignFrom(StackPlace place, ReferenceCapability referenceCapability)
        {
            if (place.Graph != Graph)
                throw new ArgumentException("Must be part of the same graph", nameof(place));

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
            if (!IsAllocated) return; // already freed
            IsAllocated = false;
            ReleaseReferences();
        }

        protected void ReleaseReferences()
        {
            foreach (var reference in references)
            {
                var deleted = false;
                if (reference.CouldHaveOwnership)
                {
                    // Delete while still in the graph
                    Graph.Delete(reference.Referent);
                    deleted = true;
                }

                // Release reference before checking if the object is reachable
                reference.Release(Graph);
                Graph.LostReference(reference.Referent);

                if (!deleted
                    && reference.Referent.GetCurrentAccess() is null) // not reachable
                    Graph.Delete(reference.Referent);
            }

            references.Clear();
        }

        protected internal void AddReference(IReference reference)
        {
            if (reference.Referent.Graph != Graph)
                throw new ArgumentException("Must be a reference to part of the same graph", nameof(reference));

            references.Add(reference);
            Graph.Dirty();
        }

        protected void AddReferences(IReadOnlyCollection<IReference> references)
        {
            if (references.Any(r => r.Referent.Graph != Graph))
                throw new ArgumentException("Must be references to parts of the same graph", nameof(references));

            this.references.AddRange(references);
            Graph.Dirty();
        }

        internal void MarkReferencedObjects()
        {
            foreach (var reference in References.Where(r => !r.IsReleased))
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
