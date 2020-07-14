using System;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
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

        public static bool CanBeAcquired(this ReferenceCapability referenceCapability)
        {
            switch (referenceCapability)
            {
                default:
                    throw ExhaustiveMatch.Failed(referenceCapability);
                case ReferenceCapability.OwnedMutable:
                case ReferenceCapability.Owned:
                case ReferenceCapability.IsolatedMutable:
                case ReferenceCapability.Isolated:
                case ReferenceCapability.HeldMutable:
                case ReferenceCapability.Held:
                    return true;
                case ReferenceCapability.Borrowed:
                case ReferenceCapability.Shared:
                case ReferenceCapability.Identity:
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
                case (ReferenceCapability.Identity, _):
                case (_, _) when target == source:
                    return true;
                case (/* Not Identity */ _, ReferenceCapability.Identity):
                    return false;
                // Anything that isn't identity can be shared
                case (ReferenceCapability.Shared, _):
                // Mutable things can be borrowed
                case (ReferenceCapability.Borrowed, ReferenceCapability.OwnedMutable):
                case (ReferenceCapability.Borrowed, ReferenceCapability.IsolatedMutable):
                case (ReferenceCapability.Borrowed, ReferenceCapability.HeldMutable):
                // Held can accept borrow/share or ownership
                case (ReferenceCapability.HeldMutable, ReferenceCapability.Borrowed):
                case (ReferenceCapability.HeldMutable, ReferenceCapability.OwnedMutable):
                case (ReferenceCapability.HeldMutable, ReferenceCapability.IsolatedMutable):
                case (ReferenceCapability.Held, ReferenceCapability.Shared):
                case (ReferenceCapability.Held, ReferenceCapability.Owned):
                case (ReferenceCapability.Held, ReferenceCapability.Isolated):
                case (ReferenceCapability.Held, ReferenceCapability.Borrowed):
                case (ReferenceCapability.Held, ReferenceCapability.OwnedMutable):
                case (ReferenceCapability.Held, ReferenceCapability.IsolatedMutable):
                // Mutable things can be weakened to read-only
                case (ReferenceCapability.Owned, ReferenceCapability.OwnedMutable):
                case (ReferenceCapability.Isolated, ReferenceCapability.IsolatedMutable):
                case (ReferenceCapability.Held, ReferenceCapability.HeldMutable):
                // Isolated can be weakened to owned
                case (ReferenceCapability.Owned, ReferenceCapability.Isolated):
                case (ReferenceCapability.OwnedMutable, ReferenceCapability.IsolatedMutable):
                // Isolated allows recovering mutability
                case (ReferenceCapability.IsolatedMutable, ReferenceCapability.Isolated):
                    return true;
                // Can't borrow from read-only
                case (ReferenceCapability.Borrowed, ReferenceCapability.Owned):
                case (ReferenceCapability.Borrowed, ReferenceCapability.Isolated):
                case (ReferenceCapability.Borrowed, ReferenceCapability.Held):
                case (ReferenceCapability.Borrowed, ReferenceCapability.Shared):
                // All other conversions to ownership disallowed
                case (ReferenceCapability.Owned, _):
                case (ReferenceCapability.OwnedMutable, _):
                case (ReferenceCapability.IsolatedMutable, _):
                case (ReferenceCapability.Isolated, _):

                case (ReferenceCapability.HeldMutable, _):
                    return false;
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
                case ReferenceCapability.OwnedMutable:
                case ReferenceCapability.IsolatedMutable:
                case ReferenceCapability.HeldMutable:
                case ReferenceCapability.Borrowed:
                    return referenceCapability;
                case ReferenceCapability.Owned:
                    return ReferenceCapability.OwnedMutable;
                case ReferenceCapability.Isolated:
                    return ReferenceCapability.IsolatedMutable;
                case ReferenceCapability.Held:
                    return ReferenceCapability.HeldMutable;
                case ReferenceCapability.Shared:
                    return ReferenceCapability.Borrowed;
                case ReferenceCapability.Identity:
                    return ReferenceCapability.Identity;
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
                case ReferenceCapability.OwnedMutable:
                    return ReferenceCapability.Owned;
                case ReferenceCapability.IsolatedMutable:
                    return ReferenceCapability.Isolated;
                case ReferenceCapability.HeldMutable:
                    return ReferenceCapability.Held;
                case ReferenceCapability.Borrowed:
                    return ReferenceCapability.Shared;
                case ReferenceCapability.Owned:
                case ReferenceCapability.Isolated:
                case ReferenceCapability.Held:
                case ReferenceCapability.Shared:
                case ReferenceCapability.Identity:
                    return referenceCapability;
            }
        }
        public static string ToSourceString(this ReferenceCapability referenceCapability)
        {
            return referenceCapability switch
            {
                ReferenceCapability.OwnedMutable => "owned mut",
                ReferenceCapability.IsolatedMutable => "iso mut",
                ReferenceCapability.HeldMutable => "held mut",
                ReferenceCapability.Borrowed => "mut",
                ReferenceCapability.Owned => "owned",
                ReferenceCapability.Isolated => "iso",
                ReferenceCapability.Held => "held",
                ReferenceCapability.Shared => "",
                ReferenceCapability.Identity => "id",
                _ => throw ExhaustiveMatch.Failed(referenceCapability)
            };
        }
    }
}
