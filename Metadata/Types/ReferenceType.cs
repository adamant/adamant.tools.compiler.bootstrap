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
        public bool IsReadOnly => !ReferenceCapability.IsMutable();
        public bool IsMutable => ReferenceCapability.IsMutable();
        public bool IsMovable => ReferenceCapability.IsMovable();

        /// <summary>
        /// Whether this type was declared `mut class` or just `class`
        /// </summary>
        public bool DeclaredMutable { get; }

        public override ValueSemantics ValueSemantics { get; }

        protected ReferenceType(bool declaredMutable, ReferenceCapability referenceCapability)
        {
            ReferenceCapability = referenceCapability;
            DeclaredMutable = declaredMutable;
            ValueSemantics = referenceCapability.GetValueSemantics();
        }

        protected internal override Self ToReadOnlyReturnsSelf()
        {
            return WithCapabilityReturnsSelf(ReferenceCapability.ToReadOnly());
        }

        protected internal abstract Self WithCapabilityReturnsSelf(ReferenceCapability referenceCapability);
    }

    public static class ReferenceTypeExtensions
    {
        /// <summary>
        /// Return the same type except with the given reference capability
        /// </summary>
        public static T WithCapability<T>(this T type, ReferenceCapability referenceCapability)
            where T : ReferenceType
        {
            return type.WithCapabilityReturnsSelf(referenceCapability).Cast<T>();
        }
    }
}
