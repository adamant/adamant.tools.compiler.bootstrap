using System;
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
                    //throw ExhaustiveMatch.Failed((target, source));
                    throw new NotImplementedException();
                case (Identity, _):
                case (_, _) when target == source:
                    return true;
                case (/* Not Identity */ _, Identity):
                    return false;
                // Anything that isn't identity can be shared
                case (Shared, _):
                // Mutable things can be borrowed
                case (Borrowed, OwnedMutable):
                case (Borrowed, IsolatedMutable):
                case (Borrowed, HeldMutable):
                // Held can accept borrow/share or ownership
                case (HeldMutable, Borrowed):
                case (HeldMutable, OwnedMutable):
                case (HeldMutable, IsolatedMutable):
                case (Held, Shared):
                case (Held, Owned):
                case (Held, Isolated):
                case (Held, Borrowed):
                case (Held, OwnedMutable):
                case (Held, IsolatedMutable):
                // Mutable things can be weakened to read-only
                case (Owned, OwnedMutable):
                case (Isolated, IsolatedMutable):
                case (Held, HeldMutable):
                // Isolated can be weakened to owned
                case (Owned, Isolated):
                case (OwnedMutable, IsolatedMutable):
                // Isolated allows recovering mutability
                case (IsolatedMutable, Isolated):
                    return true;
                // Can't borrow from read-only
                case (Borrowed, Owned):
                case (Borrowed, Isolated):
                case (Borrowed, Held):
                case (Borrowed, Shared):
                // All other conversions to ownership disallowed
                case (Owned, _):
                case (OwnedMutable, _):
                case (IsolatedMutable, _):
                case (Isolated, _):

                case (HeldMutable, _):
                    return false;
            }
        }

        public static OldValueSemantics GetValueSemantics(this ReferenceCapability referenceCapability)
        {
            switch (referenceCapability)
            {
                default:
                    throw ExhaustiveMatch.Failed(referenceCapability);
                case OwnedMutable:
                case Owned:
                case IsolatedMutable:
                case Isolated:
                    return OldValueSemantics.Own;
                case HeldMutable:
                case Borrowed:
                    return OldValueSemantics.Borrow;
                case Held:
                case Shared:
                case Identity:
                    return OldValueSemantics.Share;
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
            return referenceCapability switch
            {
                OwnedMutable => "owned mut",
                IsolatedMutable => "iso mut",
                HeldMutable => "held mut",
                Borrowed => "mut",
                Owned => "owned",
                Isolated => "iso",
                Held => "held",
                Shared => "",
                Identity => "id",
                _ => throw ExhaustiveMatch.Failed(referenceCapability)
            };
        }
    }
}
