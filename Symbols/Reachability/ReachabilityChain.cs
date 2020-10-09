using System;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Reachability
{
    [Closed(
        typeof(ReachabilityAnnotation),
        typeof(ReachableReferences))]
    public abstract class ReachabilityChain
    {
        public FixedSet<Reference> References { get; }

        protected ReachabilityChain(FixedSet<Reference> references)
        {
            if (references.Count == 0)
                throw new ArgumentException("Must not be empty", nameof(references));
            References = references;
        }
    }
}
