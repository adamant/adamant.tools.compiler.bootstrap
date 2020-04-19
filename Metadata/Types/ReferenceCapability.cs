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

        public static string ToSourceString(this ReferenceCapability referenceCapability)
        {
            switch (referenceCapability)
            {
                default:
                    throw ExhaustiveMatch.Failed(referenceCapability);
                case ReferenceCapability.OwnedMutable:
                    return "owned mut";
                case ReferenceCapability.IsolatedMutable:
                    return "iso mut";
                case ReferenceCapability.HeldMutable:
                    return "held mut";
                case ReferenceCapability.Borrowed:
                    return "mut";
                case ReferenceCapability.Owned:
                    return "owned";
                case ReferenceCapability.Isolated:
                    return "iso";
                case ReferenceCapability.Held:
                    return "held";
                case ReferenceCapability.Shared:
                    return "";
                case ReferenceCapability.Identity:
                    return "id";
            }
        }
    }
}
