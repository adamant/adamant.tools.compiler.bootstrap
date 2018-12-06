using System;
using System.Diagnostics;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// The data type of a value in an Adamant program. This includes potentially
    /// unresolved types like `UnknownType` or types containing unknown parts.
    /// </summary>
    public abstract class DataType
    {
        public static readonly UnknownType Unknown = UnknownType.Instance;
        public static readonly VoidType Void = VoidType.Instance;
        public static readonly NeverType Never = NeverType.Instance;
        public static readonly BoolType Bool = BoolType.Instance;
        public static readonly AnyType Any = AnyType.Instance;
        public static readonly TypeType Type = TypeType.Instance;
        public static readonly SizedIntegerType Int8 = SizedIntegerType.Int8;
        public static readonly SizedIntegerType Byte = SizedIntegerType.Byte;
        public static readonly SizedIntegerType Int16 = SizedIntegerType.Int16;
        public static readonly SizedIntegerType UInt16 = SizedIntegerType.UInt16;
        public static readonly SizedIntegerType Int = SizedIntegerType.Int;
        public static readonly SizedIntegerType UInt = SizedIntegerType.UInt;
        public static readonly SizedIntegerType Int64 = SizedIntegerType.Int64;
        public static readonly SizedIntegerType UInt64 = SizedIntegerType.UInt64;
        public static readonly UnsizedIntegerType Size = UnsizedIntegerType.Size;
        public static readonly UnsizedIntegerType Offset = UnsizedIntegerType.Offset;
        public static readonly FloatingPointType Float32 = FloatingPointType.Float32;
        public static readonly FloatingPointType Float = FloatingPointType.Float;
        public static readonly StringConstantType StringConstant = StringConstantType.Instance;

        public static readonly PointerType BytePointer = new PointerType(Byte);

        /// <summary>
        /// A resolved type is one that has no unknown or unresolved parts
        /// </summary>
        public abstract bool IsResolved { get; }

        [DebuggerHidden]
        public DataType AssertResolved()
        {
            if (!IsResolved)
                throw new ArgumentException($"Type {this} not resolved");

            return this;
        }

        public abstract override string ToString();
    }
}
