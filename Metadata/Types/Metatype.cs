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
        // The type this is a metatype for. Named instance because it represents
        // the single object instance that is of this metatype.
        public DataType Instance { get; }
        public override bool IsResolved => Instance.IsResolved;

        public Metatype(DataType instanceType)
            : base(Lifetimes.Lifetime.Forever)
        {
            Instance = instanceType;
        }

        public override string ToString()
        {
            switch (Instance)
            {
                case ObjectType _:
                case SimpleType _:
                case AnyType _:
                case TypeType _:
                case Metatype _:
                    // For these types we don't need parens, for others we might
                    return $"{Instance}.Type";
                default:
                    return $"({Instance}).Type";
            }
        }
    }
}
