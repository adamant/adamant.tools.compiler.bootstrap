using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// The data type of a value in an Adamant program. This includes potentially
    /// unresolved types like `UnknownType` or types containing unknown parts.
    /// </summary>
    public abstract class DataType
    {
        [NotNull] public static readonly UnknownType Unknown = UnknownType.Instance;
        [NotNull] public static readonly VoidType Void = VoidType.Instance;
        [NotNull] public static readonly NeverType Never = NeverType.Instance;
        [NotNull] public static readonly BoolType Bool = BoolType.Instance;
        [NotNull] public static readonly AnyType Any = AnyType.Instance;
        [NotNull] public static readonly TypeType Type = TypeType.Instance;
        [NotNull] public static readonly SizedIntegerType Int8 = SizedIntegerType.Int8;
        [NotNull] public static readonly SizedIntegerType Byte = SizedIntegerType.Byte;
        [NotNull] public static readonly SizedIntegerType Int16 = SizedIntegerType.Int16;
        [NotNull] public static readonly SizedIntegerType UInt16 = SizedIntegerType.UInt16;
        [NotNull] public static readonly SizedIntegerType Int = SizedIntegerType.Int;
        [NotNull] public static readonly SizedIntegerType UInt = SizedIntegerType.UInt;
        [NotNull] public static readonly SizedIntegerType Int64 = SizedIntegerType.Int64;
        [NotNull] public static readonly SizedIntegerType UInt64 = SizedIntegerType.UInt64;
        [NotNull] public static readonly UnsizedIntegerType Size = UnsizedIntegerType.Size;
        [NotNull] public static readonly UnsizedIntegerType Offset = UnsizedIntegerType.Offset;
        [NotNull] public static readonly FloatingPointType Float32 = FloatingPointType.Float32;
        [NotNull] public static readonly FloatingPointType Float = FloatingPointType.Float;
        [NotNull] public static readonly StringConstantType StringConstant = StringConstantType.Instance;

        /// <summary>
        /// A resolved type is one that has no unknown or unresolved parts
        /// </summary>
        public abstract bool IsResolved { get; }

        [NotNull]
        [DebuggerHidden]
        public DataType AssertResolved()
        {
            if (!IsResolved)
                throw new ArgumentException($"Type {this} not resolved");

            return this;
        }

        [NotNull]
        public abstract override string ToString();
    }
}
