using System;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class PrimitiveFixedIntegerType : DataType
    {
        [NotNull] public static readonly PrimitiveFixedIntegerType Int = new PrimitiveFixedIntegerType("int", -32);
        [NotNull] public static readonly PrimitiveFixedIntegerType UInt = new PrimitiveFixedIntegerType("uint", 32);
        [NotNull] public static readonly PrimitiveFixedIntegerType Byte = new PrimitiveFixedIntegerType("byte", 8);

        [NotNull] public Name Name { get; }
        public readonly bool IsSigned;
        public readonly int Bits;
        public override bool IsResolved => true;

        private PrimitiveFixedIntegerType([NotNull] string name, int bits)
        {
            Requires.NotNull(nameof(name), name);
            Name = new SimpleName(name, true);
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
