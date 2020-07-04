using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class NegateInstruction : InstructionWithResult
    {
        public Operand Operand { get; }
        public NumericType Type { get; }

        public NegateInstruction(
            Place resultPlace,
            NumericType type,
            Operand operand,
            TextSpan span,
            Scope scope)
            : base(resultPlace, span, scope)
        {
            Type = type;
            Operand = operand;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = NEG[{Type}] {Operand}";
        }
    }
}
