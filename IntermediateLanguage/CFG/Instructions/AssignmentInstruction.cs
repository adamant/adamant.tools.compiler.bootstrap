using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class AssignmentInstruction : Instruction
    {
        public Place Place { get; }
        public Operand Operand { get; }

        public AssignmentInstruction(Place place, Operand operand, TextSpan span, Scope scope)
            : base(span, scope)
        {
            Place = place;
            Operand = operand;
        }

        public override string ToInstructionString()
        {
            return $"{Place} = {Operand};";
        }
    }
}
