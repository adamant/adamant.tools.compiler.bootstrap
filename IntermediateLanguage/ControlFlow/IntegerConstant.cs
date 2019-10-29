using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class IntegerConstant : Constant
    {
        public BigInteger Value { get; }

        public IntegerConstant(BigInteger value, DataType type, TextSpan span)
            : base(type, span)
        {
            Value = value;
        }

        // Useful for debugging
        public override string ToString()
        {
            return Value + ": " + Type;
        }
    }
}
