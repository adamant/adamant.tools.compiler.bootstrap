using System;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public sealed class FixedSizeIntegerType : IntegerType
    {
        //internal new static readonly FixedSizeIntegerType Int8 = new FixedSizeIntegerType("int8", -8);
        internal new static readonly FixedSizeIntegerType Byte = new FixedSizeIntegerType(SpecialTypeName.Byte, 8);
        //internal new static readonly FixedSizeIntegerType Int16 = new FixedSizeIntegerType("int16", -16);
        //internal new static readonly FixedSizeIntegerType UInt16 = new FixedSizeIntegerType("uint16", 16);
#pragma warning disable CA1720
        internal new static readonly FixedSizeIntegerType Int = new FixedSizeIntegerType(SpecialTypeName.Int, -32);
        internal new static readonly FixedSizeIntegerType UInt = new FixedSizeIntegerType(SpecialTypeName.UInt, 32);
#pragma warning restore CA1720
        //internal new static readonly FixedSizeIntegerType Int64 = new FixedSizeIntegerType("int64", -64);
        //internal new static readonly FixedSizeIntegerType UInt64 = new FixedSizeIntegerType("uint64", 64);

        public bool IsSigned { get; }
        public int Bits { get; }
        public override bool IsKnown => true;

        private FixedSizeIntegerType(SpecialTypeName name, int bits)
            : base(name)
        {
            IsSigned = bits < 0;
            Bits = Math.Abs(bits);
        }
    }
}
