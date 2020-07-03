using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    [Closed(
        typeof(ContextObject),
        typeof(Object))]
    public abstract class HeapPlace : MemoryPlace
    {
        public ISyntax OriginSyntax { get; }
        /// <summary>
        /// This is the root reference that provides mutability from which
        /// all the others must directly or indirectly borrow.
        /// </summary>
        public Reference? OriginOfMutability { get; }
        /// <summary>
        /// Whether this object is currently mutable. Null indicates access is
        /// unknown. Deallocated places also have null access.
        /// </summary>
        public Access? CurrentAccess { get; private set; }

        protected HeapPlace(ISyntax originSyntax, Reference? originOfMutability)
        {
            OriginSyntax = originSyntax;
            OriginOfMutability = originOfMutability;
        }

        public void Capture(TempValue argument)
        {
            AddReferences(argument.StealReferences());
        }

        internal override void Free()
        {
            base.Free();
            CurrentAccess = null;
        }

        internal void ResetAccess()
        {
            CurrentAccess = null;
        }

        internal void MarkReadOnly()
        {
            if (!IsAllocated)
                return; // This object has actually been released

            if (CurrentAccess == Access.ReadOnly)
                return; // Already readonly, don't recur through it again

            CurrentAccess = Access.ReadOnly;
            // Recur to all used references which can read from their objects
            var references = References.Where(r => r.IsUsed && r.DeclaredReadable);
            foreach (var reference in references)
                reference.Referent.MarkReadOnly();
        }

        internal void MarkMutable()
        {
            if (CurrentAccess == Access.ReadOnly)
                // already marked read only
                return;

            CurrentAccess = Access.Mutable;
            MarkReferencedObjects();
        }

        internal void MarkIdentifiable()
        {
            if (!(CurrentAccess is null)) return;
            CurrentAccess = Access.Identify;
            MarkReferencedObjects();
        }

        public override string ToString()
        {
            return OriginSyntax.ToString();
        }
    }
}
