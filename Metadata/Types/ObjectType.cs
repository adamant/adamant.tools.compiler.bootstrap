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
    public class ObjectType : ReferenceType
    {
        public ISymbol Symbol { get; }
        public Name Name { get; }
        public bool DeclaredMutable { get; }

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
            bool isMutable)
            : base(Lifetime.Forever)
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
            IEnumerable<DataType> genericParameterTypes)
            : this(symbol, declaredMutable, genericParameterTypes, null, false)
        {
        }

        public ObjectType(ISymbol symbol, bool declaredMutable)
            : this(symbol, declaredMutable, null, null, false)
        {
        }

        /// <summary>
        /// Use this type as a mutable type. Only allowed if the type is declared mutable
        /// </summary>
        public ObjectType AsMutable()
        {
            return new ObjectType(Symbol, DeclaredMutable, GenericParameterTypes, GenericArguments, true);
        }

        /// <summary>
        /// Make a mutable version of this type regardless of whether it was declared
        /// mutable for use as the constructor parameter.
        /// </summary>
        public LifetimeType ForConstructor()
        {
            var objectType = new ObjectType(Symbol, DeclaredMutable, GenericParameterTypes, GenericArguments, true);
            return new LifetimeType(objectType, AnonymousLifetime.Instance);
        }

        public ObjectType WithGenericArguments(IEnumerable<DataType> genericArguments)
        {
            return new ObjectType(Symbol, DeclaredMutable, GenericParameterTypes, genericArguments, IsMutable);
        }

        public override string ToString()
        {
            if (IsMutable)
                return "mut " + Name;

            return Name.ToString();
        }
    }
}
