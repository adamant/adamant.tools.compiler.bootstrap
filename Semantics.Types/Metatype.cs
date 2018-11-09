using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
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
        // The type this is a metatype for. Named instance because it represents
        // the single object instance that is of this metatype.
        [NotNull] public DataType Instance { get; }

        public override IReadOnlyList<DataType> GenericParameterTypes { get; }
        public override IReadOnlyList<DataType> GenericArguments { get; }
        public override bool IsResolved { get; }

        public Metatype([NotNull] DataType instanceType)
        {
            Requires.NotNull(nameof(instanceType), instanceType);
            Instance = instanceType;
            IsResolved = instanceType.IsResolved;
            GenericParameterTypes = Enumerable.Empty<DataType>().ToReadOnlyList();
            GenericArguments = Enumerable.Empty<DataType>().ToReadOnlyList();
        }

        [NotNull]
        public override string ToString()
        {
            throw new System.NotImplementedException();
        }

        public DataType WithGenericArguments(IEnumerable<DataType> genericArguments)
        {
            // TODO fix implementation
            return this;
        }
    }
}
