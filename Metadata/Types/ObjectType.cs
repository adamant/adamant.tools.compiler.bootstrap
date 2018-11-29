using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    // Object types are the types created with class and struct declarations. The
    // primitive types are also object types. An object type may have generic parameters
    // that may be filled with generic arguments. An object type with generic parameters
    // but no generic arguments is an *unbound type*. One with generic arguments supplied
    // for all parameters is *a constructed type*. One with some but not all
    // arguments supplied is *partially constructed type*.
    public class ObjectType : ReferenceType
    {
        [NotNull] public ISymbol Symbol { get; }
        [NotNull] public Name Name { get; }
        public bool IsReferenceType { get; }
        public bool DeclaredMutable { get; }

        [CanBeNull, ItemNotNull] public FixedList<DataType> GenericParameterTypes { get; }
        public bool IsGeneric => GenericParameterTypes != null;
        public int? GenericArity => GenericParameterTypes?.Count;
        [CanBeNull, ItemCanBeNull] public FixedList<DataType> GenericArguments { get; }

        public bool IsMutable { get; }
        // TODO deal with the generic parameters and arguments
        public override bool IsResolved => true;

        private ObjectType(
            [NotNull] ISymbol symbol,
            bool isReferenceType,
            bool declaredMutable,
            [CanBeNull] [ItemNotNull] IEnumerable<DataType> genericParameterTypes,
            [CanBeNull] [ItemCanBeNull] IEnumerable<DataType> genericArguments)
        {
            Name = symbol.FullName;
            DeclaredMutable = declaredMutable;
            Symbol = symbol;
            var genericParameterTypesList = genericParameterTypes?.ToFixedList();
            GenericParameterTypes = genericParameterTypesList;
            var genericArgumentsList = (genericArguments ?? genericParameterTypesList?.Select(t => default(DataType)))?.ToFixedList();
            Requires.That(nameof(genericArguments), genericArgumentsList?.Count == genericParameterTypesList?.Count);
            GenericArguments = genericArgumentsList;
            IsReferenceType = isReferenceType;
            IsMutable = false;
        }

        public ObjectType(
            [NotNull] ISymbol symbol,
            bool isReferenceType,
            bool declaredMutable,
            [CanBeNull] [ItemNotNull] IEnumerable<DataType> genericParameterTypes)
            : this(symbol, isReferenceType, declaredMutable, genericParameterTypes, null)
        {
        }

        public ObjectType([NotNull] ISymbol symbol, bool isReferenceType, bool declaredMutable)
            : this(symbol, isReferenceType, declaredMutable, null, null)
        {
        }

        [NotNull]
        public override string ToString()
        {
            if (IsMutable)
                return "mut " + Name;

            return Name.ToString().NotNull();
        }

        [NotNull]
        public ObjectType WithGenericArguments(IEnumerable<DataType> genericArguments)
        {
            return new ObjectType(Symbol, IsReferenceType, DeclaredMutable, GenericParameterTypes, genericArguments);
        }
    }
}
