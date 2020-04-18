using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions
{
    public class IfInstruction : TerminatorInstruction
    {
        public Operand Condition { get; }
        public int ThenBlockNumber { get; }
        public int ElseBlockNumber { get; }

        public IfInstruction(
            Operand condition,
            int thenBlockNumber,
            int elseBlockNumber,
            TextSpan span,
            Scope scope)
            : base(span, scope)
        {
            Condition = condition;
            ThenBlockNumber = thenBlockNumber;
            ElseBlockNumber = elseBlockNumber;
        }

        public override string ToInstructionString()
        {
            return $"IF {Condition} GOTO BB #{ThenBlockNumber} ELSE GOTO BB #{ElseBlockNumber};";
        }
    }
}
