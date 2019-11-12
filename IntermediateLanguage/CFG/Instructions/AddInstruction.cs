using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class AddInstruction : InstructionWithResult
    {
        public Operand LeftOperand { get; }
        public Operand RightOperand { get; }
        public NumericType Type { get; }

        public AddInstruction(
            Place resultPlace,
            NumericType type,
            Operand leftOperand,
            Operand rightOperand,
            Scope scope)
            : base(resultPlace, TextSpan.Covering(leftOperand.Span, rightOperand.Span), scope)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
            Type = type;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = ADD.{Type} {LeftOperand}, {RightOperand}";
        }
    }
}
