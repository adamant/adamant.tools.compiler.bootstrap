using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
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
