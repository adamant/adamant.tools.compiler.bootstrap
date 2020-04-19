using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    // Object types are the types created with class and trait declarations. An
    // object type may have generic parameters that may be filled with generic
    // arguments. An object type with generic parameters but no generic arguments
    // is an *unbound type*. One with generic arguments supplied for all
    // parameters is *a constructed type*. One with some but not all arguments
    // supplied is *partially constructed type*.
    public class UserObjectType : ObjectType, IEquatable<UserObjectType>
    {
        // TODO for IsKnown, deal with the generic parameters and arguments
        public override bool IsKnown => true;

        private UserObjectType(
            Name fullName,
            bool declaredMutable,
            Mutability mutability,
            ReferenceCapability referenceCapability)
            : base(fullName, declaredMutable, mutability, referenceCapability)
        {
        }

        public static UserObjectType Declaration(
            ITypeSymbol symbol,
            bool mutable)
        {
            return new UserObjectType(
                symbol.FullName,
                mutable,
                Mutability.Immutable,
                ReferenceCapability.Shared);
        }

        public static UserObjectType Declaration(Name fullName, bool mutable)
        {
            return new UserObjectType(fullName, mutable, Mutability.Immutable, ReferenceCapability.Shared);
        }

        /// <summary>
        /// Use this type as a mutable type. Only allowed if the type is declared mutable
        /// </summary>
        public UserObjectType AsMutable()
        {
            Requires.That("DeclaredMutable", DeclaredMutable, "must be declared as a mutable type to use mutably");
            return new UserObjectType(Name, DeclaredMutable, Mutability.Mutable, ReferenceCapability);
        }

        /// <summary>
        /// Use this type as an immutable type.
        /// </summary>
        protected internal override Self AsImmutableReturnsSelf()
        {
            return new UserObjectType(Name, DeclaredMutable, Mutability.Immutable, ReferenceCapability);
        }

        /// <summary>
        /// Use this type with indeterminate mutability. Note that if it is declared immutable, then
        /// there can be no indeterminate mutability and this function returns an immutable type.
        /// </summary>
        public UserObjectType AsExplicitlyUpgradable()
        {
            if (!DeclaredMutable && Mutability == Mutability.Immutable)
                return this; // no change
            var mutability = DeclaredMutable ? Mutability.ExplicitlyUpgradable : Mutability.Immutable;
            return new UserObjectType(Name, DeclaredMutable, mutability, ReferenceCapability);
        }

        /// <summary>
        /// Use this type with indeterminate mutability. Note that if it is declared immutable, then
        /// there can be no indeterminate mutability and this function returns an immutable type.
        /// </summary>
        public UserObjectType AsImplicitlyUpgradable()
        {
            if (!DeclaredMutable && Mutability == Mutability.Immutable)
                return this; // no change
            var mutability = DeclaredMutable ? Mutability.ImplicitlyUpgradable : Mutability.Immutable;
            return new UserObjectType(Name, DeclaredMutable, mutability, ReferenceCapability);
        }

        /// <summary>
        /// Changes the lifetime to owned and if possible changes the mutability to implicitly upgradable
        /// </summary>
        public UserObjectType AsOwnedUpgradable()
        {
            var expectedMutability = DeclaredMutable ? Mutability.ImplicitlyUpgradable : Mutability.Immutable;
            //if (Lifetime == Lifetime.Owned && Mutability == expectedMutability)
            //    return this;
            return new UserObjectType(Name, DeclaredMutable, expectedMutability, ReferenceCapability.Owned);
        }

        /// <summary>
        /// Changes the lifetime to owned
        /// </summary>
        public UserObjectType AsOwned()
        {
            //if (Lifetime == Lifetime.Owned)
            //    return this;
            return new UserObjectType(Name, DeclaredMutable, Mutability, ReferenceCapability.Owned);
        }

        /// <summary>
        /// Make a mutable version of this type regardless of whether it was declared
        /// mutable for use as the constructor parameter.
        /// </summary>
        public UserObjectType ForConstructorSelf()
        {
            return new UserObjectType(Name, DeclaredMutable, Mutability.Mutable, ReferenceCapability.Borrowed);
        }

        protected internal override Self WithCapabilityReturnsSelf(ReferenceCapability referenceCapability)
        {
            return new UserObjectType(Name, DeclaredMutable, Mutability, referenceCapability);
        }

        public override string ToString()
        {
            var value = $"{Mutability}{Name}";
            //if (!(Lifetime is NoLifetime))
            //    value += "$" + Lifetime;
            return value;
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
                   && Mutability == other.Mutability
                   && ReferenceCapability == other.ReferenceCapability;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, DeclaredMutable, Mutability, ReferenceCapability);
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

        public bool EqualExceptLifetimeAndMutability(UserObjectType other)
        {
            return EqualityComparer<Name>.Default.Equals(Name, other.Name)
                   && DeclaredMutable == other.DeclaredMutable;
        }

        public override bool EqualExceptLifetime(DataType other)
        {
            return other is UserObjectType otherUserType
                    && EqualityComparer<Name>.Default.Equals(Name, otherUserType.Name)
                    && DeclaredMutable == otherUserType.DeclaredMutable
                    && Mutability == otherUserType.Mutability;
        }
    }
}
