using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions
{
    public class ReturnVoidInstruction : TerminatorInstruction
    {
        public ReturnVoidInstruction(TextSpan span, Scope scope)
            : base(span, scope)

        {
        }

        public override string ToInstructionString()
        {
            return "RETURN";
        }
    }
}
