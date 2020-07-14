using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class CompareInstruction : InstructionWithResult
    {
        public CompareInstructionOperator Operator { get; }
        public NumericType Type { get; }
        public Operand LeftOperand { get; }
        public Operand RightOperand { get; }

        public CompareInstruction(
            Place resultPlace,
            CompareInstructionOperator @operator,
            NumericType type,
            Operand leftOperand,
            Operand rightOperand,
            Scope scope)
            : base(resultPlace, TextSpan.Covering(leftOperand.Span, rightOperand.Span), scope)
        {
            Operator = @operator;
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
            Type = type;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = {Operator.ToInstructionString()}[{Type}] {LeftOperand}, {RightOperand}";
        }
    }
}
