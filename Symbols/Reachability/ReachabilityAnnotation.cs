using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Reachability
{
    public class ReachabilityAnnotation : ReachabilityChain
    {
        public ReachabilityChain CanReach { get; }

        public ReachabilityAnnotation(FixedSet<Reference> references, ReachabilityChain canReach)
            : base(references)
        {
            CanReach = canReach;
        }

        public ReachabilityAnnotation(Reference reference, ReachabilityChain canReach)
            : this(new FixedSet<Reference>(reference.Yield()), canReach) { }
    }
}
