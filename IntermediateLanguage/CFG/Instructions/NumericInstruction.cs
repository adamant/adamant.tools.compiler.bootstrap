using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class NumericInstruction : InstructionWithResult
    {
        public NumericInstructionOperator Operator { get; }

        public Operand LeftOperand { get; }
        public Operand RightOperand { get; }
        public NumericType Type { get; }

        public NumericInstruction(
            Place resultPlace,
            NumericInstructionOperator @operator,
            NumericType type,
            Operand leftOperand,
            Operand rightOperand,
            Scope scope)
            : base(resultPlace, TextSpan.Covering(leftOperand.Span, rightOperand.Span), scope)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
            Operator = @operator;
            Type = type;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = {Operator.ToInstructionString()}.{Type} {LeftOperand}, {RightOperand}";
        }
    }
}
