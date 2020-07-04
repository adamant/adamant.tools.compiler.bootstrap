using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions
{
    /// <summary>
    /// Constructs a value of an optional type from the non-optional value
    /// </summary>
    public class SomeInstruction : InstructionWithResult
    {
        public Operand Operand { get; }
        public OptionalType Type { get; }

        public SomeInstruction(Place resultPlace, OptionalType type, Operand operand, TextSpan span, Scope scope)
            : base(resultPlace, span, scope)
        {
            Type = type;
            Operand = operand;
        }

        public override string ToInstructionString()
        {
            return $"{ResultPlace} = SOME[{Type}] {Operand}";
        }
    }
}
