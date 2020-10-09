using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Reachability
{
    public class ReachableReferences : ReachabilityChain
    {
        public ReachableReferences(FixedSet<Reference> references)
            : base(references) { }

        public ReachableReferences(Reference reference)
            : this(new FixedSet<Reference>(reference.Yield())) { }
    }
}
