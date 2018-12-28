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
        public bool DeclaredMutable { get; } // TODO maybe this should be removed or computed from the symbol

        public FixedList<DataType> GenericParameterTypes { get; }
        public bool IsGeneric => GenericParameterTypes != null;
        public int? GenericArity => GenericParameterTypes?.Count;
        public FixedList<DataType> GenericArguments { get; }

        public bool IsMutable { get; }
        // TODO deal with the generic parameters and arguments
        public override bool IsResolved => true;

        private ObjectType(
            ISymbol symbol,
            bool declaredMutable,
            IEnumerable<DataType> genericParameterTypes,
            IEnumerable<DataType> genericArguments,
            bool isMutable,
            Lifetime lifetime)
            : base(lifetime)
        {
            Name = symbol.FullName;
            DeclaredMutable = declaredMutable;
            Symbol = symbol;
            var genericParameterTypesList = genericParameterTypes?.ToFixedList();
            GenericParameterTypes = genericParameterTypesList;
            var genericArgumentsList = (genericArguments ?? genericParameterTypesList?.Select(t => default(DataType)))?.ToFixedList();
            Requires.That(nameof(genericArguments), genericArgumentsList?.Count == genericParameterTypesList?.Count, "number of arguments must match number of parameters");
            GenericArguments = genericArgumentsList;
            IsMutable = isMutable;
        }

        public ObjectType(
            ISymbol symbol,
            bool declaredMutable,
            IEnumerable<DataType> genericParameterTypes,
            Lifetime lifetime)
            : this(symbol, declaredMutable, genericParameterTypes, null, false, lifetime)
        {
        }

        public ObjectType(ISymbol symbol, bool declaredMutable, Lifetime lifetime)
            : this(symbol, declaredMutable, null, null, false, lifetime)
        {
        }

        /// <summary>
        /// Use this type as a mutable type. Only allowed if the type is declared mutable
        /// </summary>
        public ObjectType AsMutable()
        {
            return new ObjectType(Symbol, DeclaredMutable, GenericParameterTypes, GenericArguments, true, Lifetime);
        }

        /// <summary>
        /// Make a mutable version of this type regardless of whether it was declared
        /// mutable for use as the constructor parameter.
        /// </summary>
        public ObjectType ForConstructor()
        {
            return new ObjectType(Symbol, DeclaredMutable, GenericParameterTypes, GenericArguments, true, AnonymousLifetime.Instance);
        }

        public ObjectType WithGenericArguments(IEnumerable<DataType> genericArguments)
        {
            return new ObjectType(Symbol, DeclaredMutable, GenericParameterTypes, genericArguments, IsMutable, Lifetime);
        }

        public override ReferenceType WithLifetime(Lifetime lifetime)
        {
            return new ObjectType(Symbol, DeclaredMutable, GenericParameterTypes, GenericArguments, IsMutable, lifetime);
        }

        public override string ToString()
        {
            var value = Name.ToString();
            if (!(Lifetime is NoLifetime)) value += "$" + Lifetime;
            if (IsMutable) value = "mut " + value;
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
                   IsMutable == other.IsMutable &&
                   EqualityComparer<Lifetime>.Default.Equals(Lifetime, other.Lifetime);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Symbol, Name, DeclaredMutable, GenericParameterTypes, GenericArguments, IsMutable, Lifetime);
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
