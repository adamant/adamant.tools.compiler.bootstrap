using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    [Closed(
        typeof(ContextObject),
        typeof(Object))]
    internal abstract class HeapPlace : Place
    {
        /// <summary>
        /// This is the root reference that provides mutability from which
        /// all the others must directly or indirectly borrow.
        /// </summary>
        public Reference? OriginOfMutability { get; }
        public ObjectState? State { get; set; }

        protected HeapPlace(Reference? originOfMutability)
        {
            OriginOfMutability = originOfMutability;
        }
    }
}
