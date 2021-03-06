using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class AssignmentInstruction : InstructionWithResult
    {
        public Operand Operand { get; }

        public AssignmentInstruction(Place resultPlace, Operand operand, TextSpan span, Scope scope)
            : base(resultPlace, span, scope)
        {
            Operand = operand;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = {Operand};";
        }
    }
}
