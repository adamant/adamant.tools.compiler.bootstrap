using System;
using System.Collections.Generic;
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
    public sealed class ObjectType : ReferenceType, IEquatable<ObjectType>
    {
        public Name FullName { get; }

        public override bool IsKnown { [DebuggerStepThrough] get => true; }

        // TODO referenceCapability needs to match declared mutable?
        public ObjectType(
            Name fullName,
            bool declaredMutable,
            ReferenceCapability referenceCapability)
            : base(declaredMutable, referenceCapability)
        {
            FullName = fullName;
        }

        /// <summary>
        /// Use this type as a mutable type. Only allowed if the type is declared mutable
        /// </summary>
        public ObjectType ToMutable()
        {
            Requires.That("DeclaredMutable", DeclaredMutable, "must be declared as a mutable type to use mutably");
            return new ObjectType(FullName, DeclaredMutable, ReferenceCapability.ToMutable());
        }

        /// <summary>
        /// Make a mutable version of this type regardless of whether it was declared
        /// mutable for use as the constructor parameter.
        /// </summary>
        public ObjectType ForConstructorSelf()
        {
            return new ObjectType(FullName, DeclaredMutable, ReferenceCapability.Borrowed);
        }

        public override string ToString()
        {
            var capability = ReferenceCapability.ToSourceString();
            if (capability.Length == 0) return FullName.ToString();
            return $"{capability} {FullName}";
        }

        #region Equality
        public override bool Equals(object? obj)
        {
            return Equals(obj as ObjectType);
        }

        public bool Equals(ObjectType? other)
        {
            return !(other is null) && EqualityComparer<Name>.Default.Equals(FullName, other.FullName)
                                    && DeclaredMutable == other.DeclaredMutable
                                    && ReferenceCapability == other.ReferenceCapability;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FullName, DeclaredMutable, ReferenceCapability);
        }

        public static bool operator ==(ObjectType type1, ObjectType type2)
        {
            return EqualityComparer<ObjectType>.Default.Equals(type1, type2);
        }

        public static bool operator !=(ObjectType type1, ObjectType type2)
        {
            return !(type1 == type2);
        }

        #endregion

        protected internal override Self To_ReturnsSelf(ReferenceCapability referenceCapability)
        {
            return new ObjectType(FullName, DeclaredMutable, referenceCapability);
        }
    }
}
