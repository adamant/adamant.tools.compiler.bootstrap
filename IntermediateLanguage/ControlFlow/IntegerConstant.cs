using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class IntegerConstant : Constant
    {
        public readonly BigInteger Value;

        public IntegerConstant(BigInteger value, [NotNull] DataType type)
            : base(type)
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
