using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions
{
    public class ReturnValueInstruction : TerminatorInstruction
    {
        public Operand Value { get; }

        public ReturnValueInstruction(Operand value, TextSpan span, Scope scope)
            : base(span, scope)
        {
            Value = value;
        }

        public override string ToInstructionString()
        {
            return $"RETURN {Value}";
        }
    }
}
