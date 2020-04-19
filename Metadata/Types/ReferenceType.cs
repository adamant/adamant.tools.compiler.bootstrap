using Adamant.Tools.Compiler.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    [Closed(
        typeof(ObjectType),
        typeof(AnyType))]
    public abstract class ReferenceType : DataType
    {
        public ReferenceCapability ReferenceCapability { get; }
        public bool IsOwned =>
            ReferenceCapability == ReferenceCapability.Owned
            || ReferenceCapability == ReferenceCapability.OwnedMutable;

        /// <summary>
        /// Whether this type was declared `mut class` or just `class`
        /// </summary>
        public bool DeclaredMutable { get; }

        public Mutability Mutability { get; }

        public override ValueSemantics ValueSemantics { get; }

        protected ReferenceType(bool declaredMutable, Mutability mutability, ReferenceCapability referenceCapability)
        {
            ReferenceCapability = referenceCapability;
            DeclaredMutable = declaredMutable;
            Mutability = mutability;
            ValueSemantics = IsOwned
                ? ValueSemantics.Own
                : (Mutability == Mutability.Mutable ? ValueSemantics.Borrow : ValueSemantics.Alias);
        }

        protected internal override Self AsDeclaredReturnsSelf()
        {
            if (!Mutability.IsUpgradable)
                return this;
            return this.AsImmutable();
        }

        /// <summary>
        /// Use this type as an immutable type.
        /// </summary>
        protected internal abstract Self AsImmutableReturnsSelf();

        protected internal abstract Self WithCapabilityReturnsSelf(ReferenceCapability referenceCapability);
    }

    public static class ReferenceTypeExtensions
    {
        public static T AsImmutable<T>(this T type)
            where T : ReferenceType
        {
            return type.AsImmutableReturnsSelf().Cast<T>();
        }

        public static T WithCapability<T>(this T type, ReferenceCapability referenceCapability)
            where T : ReferenceType
        {
            return type.WithCapabilityReturnsSelf(referenceCapability).Cast<T>();
        }
    }
}
