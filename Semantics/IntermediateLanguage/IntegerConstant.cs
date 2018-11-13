using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    public class IntegerConstant : Constant
    {
        public readonly BigInteger Value;

        public IntegerConstant(BigInteger value, [NotNull] DataType type)
            : base(type)
        {
            Requires.NotNull(nameof(type), type);
            Value = value;
        }

        // Useful for debugging
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
