using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Lifetimes;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// As in Swift, the metatype is the type of a type. That is the type of the
    /// metaobject which has the class or associated functions on it. The metatype
    /// of a class can be accessed using `Class_Name.type`. Metatypes are subtypes
    /// of `Type`.
    ///
    /// This can be very confusing. Think of types as values. Then `Class_Name`
    /// refers to an object whose type is `Class_Name.type`. Note that for any
    /// any type `T`, `T.type.type == Type`.
    ///
    /// see https://docs.swift.org/swift-book/ReferenceManual/Types.html#grammar_metatype-type
    /// </summary>
    public class Metatype : ObjectType
    {
        // The type this is a metatype for. Named instance because it represents
        // the single object instance that is of this metatype.
        public DataType Instance { get; }
        public override bool IsKnown => Instance.IsKnown;

        public Metatype(ObjectType instanceType)
            : base(new QualifiedName(instanceType.Name, SpecialName.Type),
                false, Mutability.Immutable,
                Lifetime.Forever)
        {
            Instance = instanceType;
        }

        public Metatype(SimpleType instanceType)
            : base(new QualifiedName(instanceType.Name, SpecialName.Type),
                false, Mutability.Immutable,
                Lifetime.Forever)
        {
            Instance = instanceType;
        }

        protected internal override Self AsImmutableReturnsSelf()
        {
            throw new System.NotImplementedException();
        }

        protected internal override Self WithLifetimeReturnsSelf(Lifetime lifetime)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString()
        {
            // For these types we don't need parens, for others we might
            return $"{Instance}.Type";
        }
    }
}
