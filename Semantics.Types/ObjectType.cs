using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    // Object types are the types created with class and struct declarations. The
    // primitive types are also object types. An object type may have generic parameters
    // that may be filled with generic arguments. An object type with generic parameters
    // but no generic arguments is *open* or *unbound*. One with generic arguments supplied
    // for all parameters is *closed* or *bound*. One with some but not all
    // arguments supplied is *partially bound*.
    public class ObjectType : GenericType
    {
        [NotNull] public static readonly ObjectType Void = new ObjectType(new SimpleName("void", true), false, false);
        [NotNull] public static readonly ObjectType Never = new ObjectType(new SimpleName("never", true), false, false);
        [NotNull] public static readonly ObjectType Int = new ObjectType(new SimpleName("int", true), false, false);
        [NotNull] public static readonly ObjectType UInt = new ObjectType(new SimpleName("uint", true), false, false);
        [NotNull] public static readonly ObjectType Byte = new ObjectType(new SimpleName("byte", true), false, false);
        [NotNull] public static readonly ObjectType Size = new ObjectType(new SimpleName("size", true), false, false);
        [NotNull] public static readonly ObjectType Offset = new ObjectType(new SimpleName("offset", true), false, false);
        [NotNull] public static readonly ObjectType Bool = new ObjectType(new SimpleName("bool", true), false, false);
        [NotNull] public static readonly ObjectType String = new ObjectType(new SimpleName("string", true), false, false);
        [NotNull] public static readonly ObjectType Type = new ObjectType(new SimpleName("type", true), true, false);
        [NotNull] public static readonly ObjectType Metatype = new ObjectType(new SimpleName("metatype", true), true, false);
        [NotNull] public static readonly ObjectType Any = new ObjectType(new SimpleName("any", true), true, false);

        [NotNull] public Name Name { get; }
        public bool IsReferenceType { get; }
        public bool DeclaredMutable { get; }
        [CanBeNull, ItemNotNull] public override IReadOnlyList<DataType> GenericParameterTypes { get; }
        public bool IsGeneric => GenericParameterTypes != null;
        [CanBeNull, ItemCanBeNull] public override IReadOnlyList<DataType> GenericArguments { get; }
        public bool IsMutable { get; }

        private ObjectType(
            [NotNull] Name name,
            bool isReferenceType,
            bool declaredMutable,
            [CanBeNull, ItemNotNull] IEnumerable<DataType> genericParameterTypes,
            [CanBeNull, ItemCanBeNull] IEnumerable<DataType> genericArguments)
        {
            Requires.NotNull(nameof(name), name);
            Name = name;
            DeclaredMutable = declaredMutable;
            var genericParameterTypesList = genericParameterTypes?.ToReadOnlyList();
            GenericParameterTypes = genericParameterTypesList;
            var genericArgumentsList = (genericArguments ?? genericParameterTypesList?.Select(t => default(DataType)))?.ToReadOnlyList();
            Requires.That(nameof(genericArguments), genericArgumentsList?.Count == genericParameterTypesList?.Count);
            GenericArguments = genericArgumentsList;
            IsReferenceType = isReferenceType;
            IsMutable = false;
        }

        public ObjectType(
            [NotNull] Name name,
            bool isReferenceType,
            bool declaredMutable,
            [NotNull, ItemNotNull] IEnumerable<DataType> genericParameterTypes)
            : this(name, isReferenceType, declaredMutable, genericParameterTypes, null)
        {
        }

        public ObjectType([NotNull] Name name, bool isReferenceType, bool declaredMutable)
            : this(name, isReferenceType, declaredMutable, null, null)
        {
        }

        [NotNull]
        public override string ToString()
        {
            if (IsMutable)
                return "mut " + Name;

            return Name.ToString().AssertNotNull();
        }

        [NotNull]
        public ObjectType WithGenericArguments(IEnumerable<DataType> genericArguments)
        {
            return new ObjectType(Name, IsReferenceType, DeclaredMutable, GenericParameterTypes, genericArguments);
        }
    }
}
