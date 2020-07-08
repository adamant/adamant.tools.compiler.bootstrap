using System.Diagnostics.CodeAnalysis;
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
        /// Whether this type was declared `mut class` or just `class`. Types
        /// not declared mutably are always immutable.
        /// </summary>
        public bool DeclaredMutable { get; }

        // TODO clarify this
        public override ValueSemantics ValueSemantics { get; }

        protected ReferenceType(bool declaredMutable, ReferenceCapability referenceCapability)
        {
            ReferenceCapability = referenceCapability;
            DeclaredMutable = declaredMutable;
            ValueSemantics = referenceCapability.GetValueSemantics();
        }

        protected internal sealed override Self ToReadOnly_ReturnsSelf()
        {
            return To_ReturnsSelf(ReferenceCapability.ToReadOnly());
        }

        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores",
            Justification = "Returns self idiom")]
        protected internal abstract Self To_ReturnsSelf(ReferenceCapability referenceCapability);
    }

    public static class ReferenceTypeExtensions
    {
        /// <summary>
        /// Return the same type except with the given reference capability
        /// </summary>
        public static T To<T>(this T type, ReferenceCapability referenceCapability)
            where T : ReferenceType
        {
            return type.To_ReturnsSelf(referenceCapability).Cast<T>();
        }
    }
}
