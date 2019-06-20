using System.Numerics;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// This is the type of integer constants, it isn't possible to declare a
    /// variable to have this type.
    /// </summary>
    public class IntegerConstantType : IntegerType
    {
        public readonly BigInteger Value;
        public override bool IsKnown => true;

        public IntegerConstantType(BigInteger value)
            : base(value.ToString())
        {
            Value = value;
        }
    }
}
