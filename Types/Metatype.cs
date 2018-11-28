using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    /// <summary>
    /// As in Swift, the metatype is the type of a type. That is the type of the
    /// metaobject which has the class or associated functions on it. The metatype
    /// of a class can be accessed using `Class_Name.Type`. Metatypes are subtypes
    /// of `Metatype` which is a subtype of `Type`.
    ///
    /// This can be very confusing. Think of types as values. Then `Class_Name`
    /// refers to an object whose type is `Class_Name.type`. Note that for any
    /// any type `T`, `T.Type.Type == Metatype`.
    ///
    /// see https://docs.swift.org/swift-book/ReferenceManual/Types.html#grammar_metatype-type
    /// </summary>
    public class Metatype : ReferenceType
    {
        // The type this is a metatype for. Named instance because it represents
        // the single object instance that is of this metatype.
        [NotNull] public DataType Instance { get; }
        public override bool IsResolved => Instance.IsResolved;

        public Metatype([NotNull] DataType instanceType)
        {
            Requires.NotNull(nameof(instanceType), instanceType);
            Instance = instanceType;
        }

        [NotNull]
        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
