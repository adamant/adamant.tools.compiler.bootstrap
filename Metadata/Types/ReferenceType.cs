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

        public override ValueSemantics ValueSemantics => ValueSemantics.Reference;

        /// <summary>
        /// Whether this type was declared `mut class` or just `class`. Types
        /// not declared mutably are always immutable.
        /// </summary>
        public bool DeclaredMutable { get; }

        // TODO clarify this
        public override OldValueSemantics OldValueSemantics { get; }

        private protected ReferenceType(bool declaredMutable, ReferenceCapability referenceCapability)
        {
            ReferenceCapability = referenceCapability;
            DeclaredMutable = declaredMutable;
            OldValueSemantics = referenceCapability.GetValueSemantics();
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
