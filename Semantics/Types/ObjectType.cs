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
    public class ObjectType : KnownType
    {
        [NotNull] public static readonly ObjectType Void = new ObjectType(new QualifiedName(new SimpleName("void", true)), false, false);
        [NotNull] public static readonly ObjectType Never = new ObjectType(new QualifiedName(new SimpleName("never", true)), false, false);
        [NotNull] public static readonly ObjectType Int = new ObjectType(new QualifiedName(new SimpleName("int", true)), false, false);
        [NotNull] public static readonly ObjectType UInt = new ObjectType(new QualifiedName(new SimpleName("uint", true)), false, false);
        [NotNull] public static readonly ObjectType Byte = new ObjectType(new QualifiedName(new SimpleName("byte", true)), false, false);
        [NotNull] public static readonly ObjectType Size = new ObjectType(new QualifiedName(new SimpleName("size", true)), false, false);
        [NotNull] public static readonly ObjectType Bool = new ObjectType(new QualifiedName(new SimpleName("bool", true)), false, false);
        [NotNull] public static readonly ObjectType String = new ObjectType(new QualifiedName(new SimpleName("string", true)), false, false);
        [NotNull] public static readonly ObjectType Type = new ObjectType(new QualifiedName(new SimpleName("type", true)), true, false);
        [NotNull] public static readonly ObjectType Metatype = new ObjectType(new QualifiedName(new SimpleName("metatype", true)), true, false);

        [NotNull] public QualifiedName Name { get; }
        public bool IsReferenceType { get; }
        public bool DeclaredMutable { get; }
        [NotNull] public IReadOnlyList<DataType> GenericParameterTypes { get; }
        [NotNull] public IReadOnlyList<DataType> GenericArguments { get; }
        public bool IsMutable { get; }

        private ObjectType(
            [NotNull] QualifiedName name,
            bool isReferenceType,
            bool declaredMutable,
            [NotNull][ItemNotNull] IEnumerable<DataType> genericParameterTypes,
            [CanBeNull][ItemCanBeNull] IEnumerable<DataType> genericArguments)
        {
            Requires.NotNull(nameof(name), name);
            Name = name;
            DeclaredMutable = declaredMutable;
            GenericParameterTypes = genericParameterTypes.ToReadOnlyList();
            GenericArguments = (genericArguments ?? GenericParameterTypes.Select(t => default(DataType))).ToReadOnlyList();
            Requires.That(nameof(genericArguments), GenericArguments.Count == GenericParameterTypes.Count);
            IsReferenceType = isReferenceType;
            IsMutable = false;
        }

        public ObjectType(
            [NotNull] QualifiedName name,
            bool isReferenceType,
            bool declaredMutable,
            [NotNull][ItemNotNull] IEnumerable<DataType> genericParameterTypes)
            : this(name, isReferenceType, declaredMutable, genericParameterTypes, null)
        {
        }

        public ObjectType([NotNull] QualifiedName name, bool isReferenceType, bool declaredMutable)
            : this(name, isReferenceType, declaredMutable, Enumerable.Empty<DataType>())
        {
        }

        [NotNull]
        public override string ToString()
        {
            if (IsMutable)
                return "mut " + Name;

            return Name.ToString();
        }

        [NotNull]
        public ObjectType WithGenericArguments(IEnumerable<DataType> genericArguments)
        {
            return new ObjectType(Name, IsReferenceType, DeclaredMutable, GenericParameterTypes, genericArguments);
        }
    }
}
