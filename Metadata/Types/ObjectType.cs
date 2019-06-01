using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;
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
    public class ObjectType : ReferenceType, IEquatable<ObjectType>
    {
        public ISymbol Symbol { get; }
        public Name Name { get; }
        /// <summary>
        /// Whether this type was declared `mut class` or just `class`
        /// </summary>
        public bool DeclaredMutable { get; }

        public FixedList<DataType> GenericParameterTypes { get; }
        public bool IsGeneric => GenericParameterTypes != null;
        public int? GenericArity => GenericParameterTypes?.Count;
        public FixedList<DataType> GenericArguments { get; }

        public Mutability Mutability { get; }
        // TODO deal with the generic parameters and arguments
        public override bool IsResolved => true;

        private ObjectType(
            ISymbol symbol,
            bool declaredMutable,
            IEnumerable<DataType> genericParameterTypes,
            IEnumerable<DataType> genericArguments,
            Mutability mutability,
            Lifetime lifetime)
            : base(lifetime)
        {
            Name = symbol.FullName;
            DeclaredMutable = declaredMutable;
            Mutability = mutability;
            Symbol = symbol;
            var genericParameterTypesList = genericParameterTypes?.ToFixedList();
            GenericParameterTypes = genericParameterTypesList;
            var genericArgumentsList = (genericArguments ?? genericParameterTypesList?.Select(t => default(DataType)))?.ToFixedList();
            Requires.That(nameof(genericArguments), genericArgumentsList?.Count == genericParameterTypesList?.Count, "number of arguments must match number of parameters");
            GenericArguments = genericArgumentsList;
        }

        public static ObjectType Declaration(
            ISymbol symbol,
            bool mutable,
            IEnumerable<DataType> genericParameterTypes = null)
        {
            return new ObjectType(
                symbol,
                mutable,
                genericParameterTypes,
                null, // generic arguments
                Mutability.Immutable,
                Lifetime.None);
        }

        /// <summary>
        /// Use this type as a mutable type. Only allowed if the type is declared mutable
        /// </summary>
        public ObjectType AsMutable()
        {
            Requires.That("DeclaredMutable", DeclaredMutable, "must be declared as a mutable type to use mutably");
            return new ObjectType(Symbol, DeclaredMutable, GenericParameterTypes, GenericArguments, Mutability.Mutable, Lifetime);
        }

        /// <summary>
        /// Use this type with indeterminate mutability. Note that if it is declared immutable, then
        /// there can be no indeterminate mutability and this function returns and immutable type.
        /// </summary>
        public ObjectType AsUpgradable()
        {
            if (!DeclaredMutable && Mutability == Mutability.Immutable) return this; // no change
            var mutability = DeclaredMutable ? Mutability.ExplicitlyUpgradable : Mutability.Immutable;
            return new ObjectType(Symbol, DeclaredMutable, GenericParameterTypes, GenericArguments, mutability, Lifetime);
        }

        /// <summary>
        /// Changes the lifetime to owned and if possible changes the mutability to downgradable
        /// </summary>
        /// <returns></returns>
        public ObjectType AsOwned()
        {
            var expectedMutability = DeclaredMutable ? Mutability.ImplicitlyUpgradable : Mutability.Immutable;
            if (Lifetime == Lifetime.Owned && Mutability == expectedMutability) return this;
            return new ObjectType(Symbol, DeclaredMutable, GenericParameterTypes, GenericArguments, expectedMutability, Lifetime.Owned);
        }

        /// <summary>
        /// Make a mutable version of this type regardless of whether it was declared
        /// mutable for use as the constructor parameter.
        /// </summary>
        public ObjectType ForConstructorSelf()
        {
            return new ObjectType(Symbol, DeclaredMutable, GenericParameterTypes, GenericArguments, Mutability.Mutable, AnonymousLifetime.Instance);
        }

        public ObjectType WithGenericArguments(IEnumerable<DataType> genericArguments)
        {
            return new ObjectType(Symbol, DeclaredMutable, GenericParameterTypes, genericArguments, Mutability, Lifetime);
        }

        public override ReferenceType WithLifetime(Lifetime lifetime)
        {
            return new ObjectType(Symbol, DeclaredMutable, GenericParameterTypes, GenericArguments, Mutability, lifetime);
        }

        public override string ToString()
        {
            var value = $"{Mutability}{Name}";
            if (!(Lifetime is NoLifetime)) value += "$" + Lifetime;
            return value;
        }

        #region Equality
        public override bool Equals(object obj)
        {
            return Equals(obj as ObjectType);
        }

        public bool Equals(ObjectType other)
        {
            return other != null &&
                   EqualityComparer<ISymbol>.Default.Equals(Symbol, other.Symbol) &&
                   EqualityComparer<Name>.Default.Equals(Name, other.Name) &&
                   DeclaredMutable == other.DeclaredMutable &&
                   EqualityComparer<FixedList<DataType>>.Default.Equals(GenericParameterTypes, other.GenericParameterTypes) &&
                   EqualityComparer<FixedList<DataType>>.Default.Equals(GenericArguments, other.GenericArguments) &&
                   Mutability == other.Mutability &&
                   EqualityComparer<Lifetime>.Default.Equals(Lifetime, other.Lifetime);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Symbol, Name, DeclaredMutable, GenericParameterTypes, GenericArguments, Mutability, Lifetime);
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
    }
}
