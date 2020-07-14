using System.Numerics;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    /// <summary>
    /// This is the type of integer constants, it isn't possible to declare a
    /// variable to have this type.
    /// </summary>
    public sealed class IntegerConstantType : IntegerType
    {
        public override bool IsConstant => true;
        public BigInteger Value { get; }
        public override bool IsKnown => true;

        public IntegerConstantType(BigInteger value)
            : base($"const[{value}]")
        {
            Value = value;
        }

        public override DataType ToNonConstantType()
        {
            return Int;
        }
    }
}
