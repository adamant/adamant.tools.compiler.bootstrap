using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    [Closed(
        typeof(Object))]
    public abstract class HeapPlace : MemoryPlace
    {
        private Access? currentAccess;
        public IAbstractSyntax OriginSyntax { get; }
        /// <summary>
        /// This is the root reference that provides mutability from which
        /// all the others must directly or indirectly borrow.
        /// </summary>
        public IReference? OriginOfMutability { get; }

        /// <summary>
        /// Whether this object is currently mutable. Null indicates access is
        /// unknown. Deallocated places also have null access.
        /// </summary>
        public Access? GetCurrentAccess()
        {
            Graph.EnsureCurrentAccessIsUpToDate();
            return currentAccess;
        }

        private protected HeapPlace(
             IReferenceGraph graph,
             IAbstractSyntax originSyntax,
             IReference? originOfMutability)
             : base(graph)
        {
            OriginSyntax = originSyntax;
            OriginOfMutability = originOfMutability;
        }

        public void Capture(TempValue argument)
        {
            if (argument.Graph != Graph)
                throw new ArgumentException("Must be part of the same graph", nameof(argument));

            AddReferences(argument.StealReferences());
            Graph.Drop(argument);
        }

        internal override void Freed()
        {
            base.Freed();
            currentAccess = null;
        }

        #region Update Current Access
        internal void ResetAccess()
        {
            currentAccess = null;
        }

        internal void MarkReadOnly()
        {
            if (!IsAllocated) return; // This object has actually been released

            if (currentAccess == Access.ReadOnly) return; // Already readonly, don't recur through it again

            currentAccess = Access.ReadOnly;
            // Recur to all used references which can read from their objects
            var references = References.Where(r => r.IsUsed && r.DeclaredReadable);
            foreach (var reference in references) reference.Referent.MarkReadOnly();
        }

        internal void MarkMutable()
        {
            if (currentAccess == Access.ReadOnly)
                // already marked read only
                return;

            currentAccess = Access.Mutable;
            MarkReferencedObjects();
        }

        internal void MarkIdentifiable()
        {
            if (!(currentAccess is null)) return;
            currentAccess = Access.Identify;
            MarkReferencedObjects();
        }
        #endregion

        public override string ToString()
        {
            return OriginSyntax.ToString();
        }
    }
}
