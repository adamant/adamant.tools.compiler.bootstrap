using ExhaustiveMatching;
using static Adamant.Tools.Compiler.Bootstrap.Metadata.Types.ReferenceCapability;

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
                case OwnedMutable:
                case IsolatedMutable:
                case HeldMutable:
                case Borrowed:
                    return true;
                case Owned:
                case Isolated:
                case Held:
                case Shared:
                case Identity:
                    return false;
            }
        }

        public static bool IsMovable(this ReferenceCapability referenceCapability)
        {
            switch (referenceCapability)
            {
                default:
                    throw ExhaustiveMatch.Failed(referenceCapability);
                case OwnedMutable:
                case Owned:
                case IsolatedMutable:
                case Isolated:
                case HeldMutable:
                case Held:
                    return true;
                case Borrowed:
                case Shared:
                case Identity:
                    return false;
            }
        }

        public static bool IsAssignableFrom(this ReferenceCapability target, ReferenceCapability source)
        {
            switch (target, source)
            {
                default:
                    throw ExhaustiveMatch.Failed((target, source));
                case (Identity, _):
                case (_, _) when target == source:
                    return true;
                case (/* Not Identity */ _, Identity):
                    return false;
                case (Shared, _):
                case (Owned, OwnedMutable):
                case (Isolated, IsolatedMutable):
                case (Held, HeldMutable):
                case (Borrowed, Borrowed):
                case (Borrowed, OwnedMutable):
                case (Borrowed, IsolatedMutable):
                case (Borrowed, HeldMutable):
                    return true;
                case (Borrowed, Owned):
                case (Borrowed, Isolated):
                case (Borrowed, Held):
                case (Borrowed, Shared):
                case (Owned, _):
                case (OwnedMutable, _):
                case (IsolatedMutable, _):
                case (Isolated, _):
                case (HeldMutable, _):
                    return false;
            }
        }

        public static ValueSemantics GetValueSemantics(this ReferenceCapability referenceCapability)
        {
            switch (referenceCapability)
            {
                default:
                    throw ExhaustiveMatch.Failed(referenceCapability);
                case OwnedMutable:
                case Owned:
                case IsolatedMutable:
                case Isolated:
                    return Types.ValueSemantics.Own;
                case HeldMutable:
                case Borrowed:
                    return ValueSemantics.Borrow;
                case Held:
                case Shared:
                case Identity:
                    return ValueSemantics.Share;
            }
        }

        /// <summary>
        /// Convert to the equivalent reference capability that is mutable.
        /// </summary>
        public static ReferenceCapability ToMutable(this ReferenceCapability referenceCapability)
        {
            switch (referenceCapability)
            {
                default:
                    throw ExhaustiveMatch.Failed(referenceCapability);
                case OwnedMutable:
                case IsolatedMutable:
                case HeldMutable:
                case Borrowed:
                    return referenceCapability;
                case Owned:
                    return OwnedMutable;
                case Isolated:
                    return IsolatedMutable;
                case Held:
                    return HeldMutable;
                case Shared:
                    return Borrowed;
                case Identity:
                    return Identity;
            }
        }
        /// <summary>
        /// Convert to the equivalent reference capability that is read-only.
        /// </summary>
        public static ReferenceCapability ToReadOnly(this ReferenceCapability referenceCapability)
        {
            switch (referenceCapability)
            {
                default:
                    throw ExhaustiveMatch.Failed(referenceCapability);
                case OwnedMutable:
                    return Owned;
                case IsolatedMutable:
                    return Isolated;
                case HeldMutable:
                    return Held;
                case Borrowed:
                    return Shared;
                case Owned:
                case Isolated:
                case Held:
                case Shared:
                case Identity:
                    return referenceCapability;
            }
        }
        public static string ToSourceString(this ReferenceCapability referenceCapability)
        {
            switch (referenceCapability)
            {
                default:
                    throw ExhaustiveMatch.Failed(referenceCapability);
                case OwnedMutable:
                    return "owned mut";
                case IsolatedMutable:
                    return "iso mut";
                case HeldMutable:
                    return "held mut";
                case Borrowed:
                    return "mut";
                case Owned:
                    return "owned";
                case Isolated:
                    return "iso";
                case Held:
                    return "held";
                case Shared:
                    return "";
                case Identity:
                    return "id";
            }
        }
    }
}
