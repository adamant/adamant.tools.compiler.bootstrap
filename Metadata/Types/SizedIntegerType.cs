using System;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class SizedIntegerType : IntegerType
    {
        //internal new static readonly SizedIntegerType Int8 = new SizedIntegerType("int8", -8);
        internal new static readonly SizedIntegerType Byte = new SizedIntegerType("byte", 8);
        //internal new static readonly SizedIntegerType Int16 = new SizedIntegerType("int16", -16);
        //internal new static readonly SizedIntegerType UInt16 = new SizedIntegerType("uint16", 16);
#pragma warning disable CA1720
        internal new static readonly SizedIntegerType Int = new SizedIntegerType("int", -32);
        internal new static readonly SizedIntegerType UInt = new SizedIntegerType("uint", 32);
#pragma warning restore CA1720
        //internal new static readonly SizedIntegerType Int64 = new SizedIntegerType("int64", -64);
        //internal new static readonly SizedIntegerType UInt64 = new SizedIntegerType("uint64", 64);

        public bool IsSigned { get; }
        public int Bits { get; }
        public override bool IsKnown => true;

        private SizedIntegerType(string name, int bits)
            : base(name)
        {
            IsSigned = bits < 0;
            Bits = Math.Abs(bits);
        }
    }
}
