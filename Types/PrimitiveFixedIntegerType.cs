using System;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public class PrimitiveFixedIntegerType : PrimitiveType
    {
        [NotNull] public static readonly PrimitiveFixedIntegerType Int = new PrimitiveFixedIntegerType("int", -32);
        [NotNull] public static readonly PrimitiveFixedIntegerType UInt = new PrimitiveFixedIntegerType("uint", 32);
        [NotNull] public static readonly PrimitiveFixedIntegerType Byte = new PrimitiveFixedIntegerType("byte", 8);

        public readonly bool IsSigned;
        public readonly int Bits;
        public override bool IsResolved => true;

        private PrimitiveFixedIntegerType([NotNull] string name, int bits)
            : base(name)
        {
            IsSigned = bits < 0;
            Bits = Math.Abs(bits);
        }

        [NotNull]
        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
