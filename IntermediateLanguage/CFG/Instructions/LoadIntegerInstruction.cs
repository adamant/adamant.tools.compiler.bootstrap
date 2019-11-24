using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class LoadIntegerInstruction : InstructionWithResult
    {
        public BigInteger Value { get; }
        public IntegerType Type { get; }

        public LoadIntegerInstruction(Place resultPlace, BigInteger value, IntegerType type, TextSpan span, Scope scope)
            : base(resultPlace, span, scope)
        {
            Value = value;
            Type = type;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = LOAD.{Type} {Value}";
        }
    }
}
