using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    /// <summary>
    /// As in Swift, the metatype is the type of a type. That is the type of the
    /// metaobject which has the class or associated functions on it. The metatype
    /// of a class can be accessed using `Class_Name.type`. Metatypes are subtypes
    /// of `metatype` which is a subtype of `type`.
    ///
    /// This can be very confusing. Think of types as values. Then `Class_Name`
    /// refers to an object whose type is `Class_Name.type`. Note that for any
    /// any type `T`, `T.type.type == metatype`.
    ///
    /// see https://docs.swift.org/swift-book/ReferenceManual/Types.html#grammar_metatype-type
    /// </summary>
    public class Metatype : GenericType
    {
        [NotNull] public Name Name { get; }
        // The type this is a metatype for. Named instance because it represents
        // the single object instance that is of this metatype.
        [NotNull] public ObjectType Instance { get; }

        public override IReadOnlyList<DataType> GenericParameterTypes => Instance.GenericParameterTypes;
        public override IReadOnlyList<DataType> GenericArguments => Instance.GenericArguments;
        public override bool IsResolved { get; }

        public Metatype([NotNull]ObjectType objectType)
        {
            Requires.NotNull(nameof(objectType), objectType);
            Name = objectType.Name;
            Instance = objectType;
            IsResolved = objectType.IsResolved;
        }

        [NotNull]
        public override string ToString()
        {
            throw new System.NotImplementedException();
        }

        public DataType WithGenericArguments(IEnumerable<DataType> genericArguments)
        {
            return new Metatype(Instance.WithGenericArguments(genericArguments));
        }
    }
}
