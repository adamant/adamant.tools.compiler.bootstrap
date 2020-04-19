using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// The data type of a value in an Adamant program. This includes potentially
    /// unresolved types like `UnknownType` or types containing unknown parts.
    /// </summary>
    [Closed(
        typeof(ReferenceType),
        typeof(ValueType),
        typeof(EmptyType),
        typeof(UnknownType)
        )]
    public abstract class DataType
    {
        #region Standard Types
        public static readonly UnknownType Unknown = UnknownType.Instance;
        public static readonly VoidType Void = VoidType.Instance;
        public static readonly NeverType Never = NeverType.Instance;
        public static readonly BoolType Bool = BoolType.Instance;
        public static readonly SizedIntegerType Byte = SizedIntegerType.Byte;
#pragma warning disable CA1720
        public static readonly SizedIntegerType Int = SizedIntegerType.Int;
        public static readonly SizedIntegerType UInt = SizedIntegerType.UInt;
#pragma warning restore CA1720
        public static readonly UnsizedIntegerType Size = UnsizedIntegerType.Size;
        public static readonly UnsizedIntegerType Offset = UnsizedIntegerType.Offset;

        //public static readonly PointerType BytePointer = new PointerType(Byte);
        /// <summary>
        /// The value `none` has this type, which is `never?`
        /// </summary>
        public static readonly OptionalType None = new OptionalType(Never);
        #endregion

        /// <summary>
        /// The `never` and `void` types are the only empty types. This means
        /// there are no values of either type. The `never` type is defined
        /// as the type without values. The `void` type behaves more like a unit
        /// type. However, its implementation is that it doesn't have a value
        /// and represents the lack of that value. For example, that a function
        /// doesn't return a value or that an argument is to be dropped.
        /// </summary>
        public virtual bool IsEmpty => false;

        /// <summary>
        /// A known type is one that has no unknown parts
        /// </summary>
        public abstract bool IsKnown { get; }

        /// <summary>
        /// The value semantics of expressions producing this type
        /// </summary>
        public abstract ValueSemantics ValueSemantics { get; }

        [DebuggerHidden]
        public DataType AssertKnown()
        {
            if (!IsKnown)
                throw new InvalidOperationException($"Type {this} not known");

            return this;
        }

        /// <summary>
        /// Returns a version of this type that could be used as a declared type.
        /// This means that upgradable mutability is converted to non-upgradable.
        /// </summary>
        protected internal virtual Self AsDeclaredReturnsSelf()
        {
            return this;
        }

        public virtual bool EqualExceptLifetime(DataType other)
        {
            return Equals(other);
        }

        public abstract override string ToString();

        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Return self idiom")]
        public static implicit operator Self(DataType type)
        {
            return new Self(type);
        }
    }

    public static class DataTypeExtensions
    {
        public static T AsDeclared<T>(this T type)
            where T : DataType
        {
            return type.AsDeclaredReturnsSelf().Cast<T>();
        }
    }
}
