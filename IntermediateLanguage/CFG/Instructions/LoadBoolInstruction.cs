using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    public class LoadBoolInstruction : InstructionWithResult
    {
        public bool Value { get; }

        public LoadBoolInstruction(Place resultPlace, bool value, TextSpan span, Scope scope)
            : base(resultPlace, span, scope)
        {
            Value = value;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = LOAD[{DataType.Bool}] {Value}";
        }
    }
}
