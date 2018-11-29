using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// As in Swift, the metatype is the type of a type. That is the type of the
    /// metaobject which has the class or associated functions on it. The metatype
    /// of a class can be accessed using `Class_Name.Type`. Metatypes are subtypes
    /// of `Type`.
    ///
    /// This can be very confusing. Think of types as values. Then `Class_Name`
    /// refers to an object whose type is `Class_Name.type`. Note that for any
    /// any type `T`, `T.Type.Type == Type`.
    ///
    /// see https://docs.swift.org/swift-book/ReferenceManual/Types.html#grammar_metatype-type
    /// </summary>
    public class Metatype : ReferenceType
    {
        [NotNull] public ISymbol Symbol { get; }
        // The type this is a metatype for. Named instance because it represents
        // the single object instance that is of this metatype.
        [NotNull] public DataType Instance { get; }
        public override bool IsResolved => Instance.IsResolved;

        public Metatype([NotNull] ISymbol symbol, [NotNull] DataType instanceType)
        {
            Instance = instanceType.NotNull();
            Symbol = symbol.NotNull();
        }

        [NotNull]
        public override string ToString()
        {
            return $"({Instance}).Type";
        }
    }
}
