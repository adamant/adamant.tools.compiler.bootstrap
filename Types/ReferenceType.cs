using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    [Closed(
        typeof(ObjectType),
        typeof(AnyType))]
    public abstract class ReferenceType : DataType
    {
        public ReferenceCapability ReferenceCapability { get; }
        public bool IsReadOnly => !ReferenceCapability.IsMutable();
        public bool IsMutable => ReferenceCapability.IsMutable();
        public bool IsMovable => ReferenceCapability.CanBeAcquired();

        public override TypeSemantics Semantics => TypeSemantics.Reference;

        /// <summary>
        /// Whether this type was declared `mut class` or just `class`. Types
        /// not declared mutably are always immutable.
        /// </summary>
        public bool DeclaredMutable { get; }

        // TODO clarify this

        private protected ReferenceType(bool declaredMutable, ReferenceCapability referenceCapability)
        {
            ReferenceCapability = referenceCapability;
            DeclaredMutable = declaredMutable;
        }

        protected internal sealed override Self ToReadOnly_ReturnsSelf()
        {
            return To_ReturnsSelf(ReferenceCapability.ToReadOnly());
        }

        [SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores",
            Justification = "Returns self idiom")]
        protected internal abstract Self To_ReturnsSelf(ReferenceCapability referenceCapability);
    }
}
