using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class LoadNoneInstruction : InstructionWithResult
    {
        public OptionalType Type { get; }

        public LoadNoneInstruction(Place resultPlace, OptionalType type, TextSpan span, Scope scope)
            : base(resultPlace, span, scope)
        {
            Type = type;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = LOAD.{Type} none";
        }
    }
}
