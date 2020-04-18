using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions
{
    public class GotoInstruction : TerminatorInstruction
    {
        public int BlockNumber { get; }

        public GotoInstruction(int blockNumber, TextSpan span, Scope scope)
            : base(span, scope)
        {
            BlockNumber = blockNumber;
        }

        public override string ToInstructionString()
        {
            return $"GOTO BB #{BlockNumber}";
        }
    }
}
