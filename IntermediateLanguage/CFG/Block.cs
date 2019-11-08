using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Block
    {
        public int Number { get; }
        public FixedList<Instruction> Instructions { get; }
        public TerminatorInstruction Terminator { get; }


        public Block(int number, FixedList<Instruction> instructions, TerminatorInstruction terminator)
        {
            Number = number;
            Instructions = instructions;
            Terminator = terminator;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"BB #{Number}, Instructions={Instructions.Count+1}";
    }
}
