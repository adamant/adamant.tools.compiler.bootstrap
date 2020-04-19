using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public enum ReferenceCapability
    {
        Owned,
        OwnedMutable,
        Isolated,
        IsolatedMutable,
        Held,
        HeldMutable,
        Shared,
        Borrowed,
        Identity,
    }

    public static class ReferenceCapabilityExtensions
    {
        public static bool IsMutable(this ReferenceCapability referenceCapability)
        {
            switch (referenceCapability)
            {
                default:
                    throw ExhaustiveMatch.Failed(referenceCapability);
                case ReferenceCapability.OwnedMutable:
                case ReferenceCapability.IsolatedMutable:
                case ReferenceCapability.HeldMutable:
                case ReferenceCapability.Borrowed:
                    return true;
                case ReferenceCapability.Owned:
                case ReferenceCapability.Isolated:
                case ReferenceCapability.Held:
                case ReferenceCapability.Shared:
                case ReferenceCapability.Identity:
                    return false;
            }
        }
    }
}
