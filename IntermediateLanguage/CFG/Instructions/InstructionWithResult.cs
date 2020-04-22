using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    [Closed(
        typeof(AssignmentInstruction),
        typeof(CompareInstruction),
        typeof(ConvertInstruction),
        typeof(FieldAccessInstruction),
        typeof(LoadBoolInstruction),
        typeof(LoadIntegerInstruction),
        typeof(LoadStringInstruction),
        typeof(LoadNoneInstruction),
        typeof(NegateInstruction),
        typeof(NewObjectInstruction),
        typeof(NumericInstruction))]
    public abstract class InstructionWithResult : Instruction
    {
        public Place ResultPlace { get; }

        protected InstructionWithResult(Place resultPlace, TextSpan span, Scope scope)
            : base(span, scope)
        {
            ResultPlace = resultPlace;
        }
    }
}
