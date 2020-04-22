using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ILGen
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    internal class BlockBuilder
    {
        public int Number { get; }
        private readonly List<Instruction> instructions = new List<Instruction>();
        public IReadOnlyList<Instruction> Instructions => instructions;
        public bool IsTerminated => Terminator != null;
        [DisallowNull]
        public TerminatorInstruction? Terminator { get; private set; }

        public BlockBuilder(in int number)
        {
            Number = number;
        }

        public void Add(Instruction instruction)
        {
            instructions.Add(instruction);
        }

        public void End(TerminatorInstruction instruction)
        {
            if (Terminator != null)
                throw new InvalidOperationException("Can't set terminator instruction more than once");
            Terminator = instruction;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"BB #{Number}, Instructions={Instructions.Count + (IsTerminated ? 1 : 0)}, Terminated={IsTerminated}";
    }
}
