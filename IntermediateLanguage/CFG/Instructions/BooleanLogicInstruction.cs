using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class BooleanLogicInstruction : InstructionWithResult
    {
        public BooleanLogicOperator Operator { get; }
        public Operand LeftOperand { get; }
        public Operand RightOperand { get; }

        public BooleanLogicInstruction(
            Place resultPlace,
            BooleanLogicOperator @operator,
            Operand leftOperand,
            Operand rightOperand,
            Scope scope)
            : base(resultPlace, TextSpan.Covering(leftOperand.Span, rightOperand.Span), scope)
        {
            Operator = @operator;
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = {Operator.ToInstructionString()} {LeftOperand}, {RightOperand}";
        }
    }
}
