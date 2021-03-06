using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;
using static Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph.Access;
using static Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph.Ownership;
using static Adamant.Tools.Compiler.Bootstrap.Types.ReferenceCapability;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal static class ReferenceCapabilityExtensions
    {
        internal static Access ToAccess(this ReferenceCapability capability)
        {
            switch (capability)
            {
                default:
                    throw ExhaustiveMatch.Failed(capability);
                case IsolatedMutable:
                case OwnedMutable:
                case HeldMutable:
                case Borrowed:
                    return Mutable;
                case Isolated:
                case Owned:
                case Held:
                case Shared:
                    return ReadOnly;
                case Identity:
                    return Identify;
            }
        }

        internal static Ownership ToOwnership(this ReferenceCapability capability)
        {
            switch (capability)
            {
                default:
                    throw ExhaustiveMatch.Failed(capability);
                case IsolatedMutable:
                case Isolated:
                case OwnedMutable:
                case Owned:
                    return Owns;
                case HeldMutable:
                case Held:
                    return PotentiallyOwns;
                case Borrowed:
                case Shared:
                case Identity:
                    return None;
            }
        }
    }
}
