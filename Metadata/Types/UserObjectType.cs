using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// Object types are the types created with class and trait declarations. An
    /// object type may have generic parameters that may be filled with generic
    /// arguments. An object type with generic parameters but no generic arguments
    /// is an *unbound type*. One with generic arguments supplied for all
    /// parameters is *a constructed type*. One with some but not all arguments
    /// supplied is *partially constructed type*.
    /// </summary>
    public class UserObjectType : ObjectType, IEquatable<UserObjectType>
    {
        // TODO for IsKnown, deal with the generic parameters and arguments
        public override bool IsKnown => true;

        private UserObjectType(
            Name fullName,
            bool declaredMutable,
            ReferenceCapability referenceCapability)
            : base(fullName, declaredMutable, referenceCapability)
        {
        }

        public static UserObjectType Declaration(
            ITypeSymbol symbol,
            bool mutable)
        {
            return new UserObjectType(
                symbol.FullName,
                mutable,
                ReferenceCapability.Shared);
        }

        public static UserObjectType Declaration(Name fullName, bool mutable)
        {
            return new UserObjectType(fullName, mutable, ReferenceCapability.Shared);
        }

        /// <summary>
        /// Use this type as a mutable type. Only allowed if the type is declared mutable
        /// </summary>
        public UserObjectType ToMutable()
        {
            Requires.That("DeclaredMutable", DeclaredMutable, "must be declared as a mutable type to use mutably");
            return new UserObjectType(Name, DeclaredMutable, ReferenceCapability.ToMutable());
        }

        /// <summary>
        /// Use this type as an immutable type.
        /// </summary>
        protected internal override Self ToReadOnlyReturnsSelf()
        {
            return new UserObjectType(Name, DeclaredMutable, ReferenceCapability);
        }

        /// <summary>
        /// Use this type with indeterminate mutability. Note that if it is declared immutable, then
        /// there can be no indeterminate mutability and this function returns an immutable type.
        /// </summary>
        //public UserObjectType AsExplicitlyUpgradable()
        //{
        //    //if (!DeclaredMutable && Mutability == Mutability.Immutable)
        //    //    return this; // no change
        //    //var mutability = DeclaredMutable ? Mutability.ExplicitlyUpgradable : Mutability.Immutable;
        //    return new UserObjectType(Name, DeclaredMutable, ReferenceCapability);
        //}

        /// <summary>
        /// Changes the lifetime to owned and if possible changes the mutability to implicitly upgradable
        /// </summary>
        //public UserObjectType AsOwnedUpgradable()
        //{
        //    //var expectedMutability = DeclaredMutable ? Mutability.ImplicitlyUpgradable : Mutability.Immutable;
        //    //if (Lifetime == Lifetime.Owned && Mutability == expectedMutability)
        //    //    return this;
        //    return new UserObjectType(Name, DeclaredMutable, ReferenceCapability.Owned);
        //}

        /// <summary>
        /// Make a mutable version of this type regardless of whether it was declared
        /// mutable for use as the constructor parameter.
        /// </summary>
        public UserObjectType ForConstructorSelf()
        {
            return new UserObjectType(Name, DeclaredMutable, ReferenceCapability.Borrowed);
        }

        public override string ToString()
        {
            var capability = ReferenceCapability.ToSourceString();
            if (capability.Length == 0) return Name.ToString();
            return $"{capability} {Name}";
        }

        #region Equality
        public override bool Equals(object? obj)
        {
            return Equals(obj as UserObjectType);
        }

        public bool Equals(UserObjectType? other)
        {
            return !(other is null)
                   && EqualityComparer<Name>.Default.Equals(Name, other.Name)
                   && DeclaredMutable == other.DeclaredMutable
                   && ReferenceCapability == other.ReferenceCapability;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, DeclaredMutable, ReferenceCapability);
        }

        public static bool operator ==(UserObjectType type1, UserObjectType type2)
        {
            return EqualityComparer<UserObjectType>.Default.Equals(type1, type2);
        }

        public static bool operator !=(UserObjectType type1, UserObjectType type2)
        {
            return !(type1 == type2);
        }
        #endregion

        //public bool EqualExceptLifetimeAndMutability(UserObjectType other)
        //{
        //    return EqualityComparer<Name>.Default.Equals(Name, other.Name)
        //           && DeclaredMutable == other.DeclaredMutable;
        //}

        //public override bool EqualExceptLifetime(DataType other)
        //{
        //    return other is UserObjectType otherUserType
        //            && EqualityComparer<Name>.Default.Equals(Name, otherUserType.Name)
        //            && DeclaredMutable == otherUserType.DeclaredMutable
        //            && Mutability == otherUserType.Mutability;
        //}

        protected internal override Self WithCapabilityReturnsSelf(ReferenceCapability referenceCapability)
        {
            return new UserObjectType(Name, DeclaredMutable, referenceCapability);
        }
    }
}
