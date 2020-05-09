using Adamant.Tools.Compiler.Bootstrap.AST;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    [Closed(
        typeof(ContextObject),
        typeof(Object))]
    internal abstract class HeapPlace : Place
    {
        public ISyntax OriginSyntax { get; }
        /// <summary>
        /// This is the root reference that provides mutability from which
        /// all the others must directly or indirectly borrow.
        /// </summary>
        public Reference? OriginOfMutability { get; }
        public ObjectState? State { get; set; }

        protected HeapPlace(ISyntax originSyntax, Reference? originOfMutability)
        {
            OriginSyntax = originSyntax;
            OriginOfMutability = originOfMutability;
        }

        public void Capture(TempValue argument)
        {
            foreach (var reference in argument.References)
                references.Add(reference);

            argument.ClearReferences();
        }
    }
}
