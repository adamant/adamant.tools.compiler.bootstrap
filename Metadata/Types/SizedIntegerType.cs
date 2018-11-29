using System;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class SizedIntegerType : IntegerType
    {
        [NotNull] internal static readonly SizedIntegerType Int8 = new SizedIntegerType("int8", -8);
        [NotNull] internal static readonly SizedIntegerType Byte = new SizedIntegerType("byte", 8);
        [NotNull] internal static readonly SizedIntegerType Int16 = new SizedIntegerType("int16", -16);
        [NotNull] internal static readonly SizedIntegerType UInt16 = new SizedIntegerType("uint16", 16);
        [NotNull] internal static readonly SizedIntegerType Int = new SizedIntegerType("int", -32);
        [NotNull] internal static readonly SizedIntegerType UInt = new SizedIntegerType("uint", 32);
        [NotNull] internal static readonly SizedIntegerType Int64 = new SizedIntegerType("int64", -64);
        [NotNull] internal static readonly SizedIntegerType UInt64 = new SizedIntegerType("uint64", 64);

        public readonly bool IsSigned;
        public readonly int Bits;
        public override bool IsResolved => true;

        private SizedIntegerType([NotNull] string name, int bits)
            : base(name)
        {
            IsSigned = bits < 0;
            Bits = Math.Abs(bits);
        }
    }
}
