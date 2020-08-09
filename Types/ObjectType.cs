using System;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    /// <summary>
    /// Object types are the types created with class and trait declarations. An
    /// object type may have generic parameters that may be filled with generic
    /// arguments. An object type with generic parameters but no generic arguments
    /// is an *unbound type*. One with generic arguments supplied for all
    /// parameters is *a constructed type*. One with some but not all arguments
    /// supplied is *partially constructed type*.
    /// </summary>
    /// <remarks>
    /// There will be two special object types `Type` and `Metatype`
    /// </remarks>
    public sealed class ObjectType : ReferenceType
    {
        public NamespaceName ContainingNamespace { get; }
        public TypeName Name { get; }
        public MaybeQualifiedName FullName => ContainingNamespace.ToRootName().Qualify(Name.ToSimpleName());

        public override bool IsKnown { [DebuggerStepThrough] get => true; }

        // TODO referenceCapability needs to match declared mutable?
        public ObjectType(
            NamespaceName containingNamespace,
            TypeName name,
            bool declaredMutable,
            ReferenceCapability referenceCapability)
            : base(declaredMutable, referenceCapability)
        {
            ContainingNamespace = containingNamespace;
            Name = name;
        }

        /// <summary>
        /// Use this type as a mutable type. Only allowed if the type is declared mutable
        /// </summary>
        public ObjectType ToMutable()
        {
            Requires.That("DeclaredMutable", DeclaredMutable, "must be declared as a mutable type to use mutably");
            return new ObjectType(ContainingNamespace, Name, DeclaredMutable, ReferenceCapability.ToMutable());
        }

        /// <summary>
        /// Make a mutable version of this type regardless of whether it was declared
        /// mutable for use as the constructor parameter.
        /// </summary>
        public ObjectType ForConstructorSelf()
        {
            return new ObjectType(ContainingNamespace, Name, DeclaredMutable, ReferenceCapability.Borrowed);
        }

        public override string ToString()
        {
            var capability = ReferenceCapability.ToSourceString();
            if (capability.Length == 0) return FullName.ToString();
            return $"{capability} {FullName}";
        }

        #region Equality
        public override bool Equals(DataType? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is ObjectType otherType
                && FullName == otherType.FullName
                && DeclaredMutable == otherType.DeclaredMutable
                && ReferenceCapability == otherType.ReferenceCapability;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FullName, DeclaredMutable, ReferenceCapability);
        }
        #endregion

        protected internal override Self To_ReturnsSelf(ReferenceCapability referenceCapability)
        {
            return new ObjectType(ContainingNamespace, Name, DeclaredMutable, referenceCapability);
        }
    }
}
