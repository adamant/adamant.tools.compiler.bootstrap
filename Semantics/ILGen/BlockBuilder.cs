using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Instructions;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Operands;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.Places;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG.TerminatorInstructions;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ILGen
{
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

        //public void Add(Statement statement)
        //{
        //    // We have to clone the statement because the statement number will
        //    // be assigned into it. Each block needs to have separate statements.
        //    instructions.Add(statement.Clone());
        //}

        public void AddAssignment(Place place, Operand operand, TextSpan span, Scope scope)
        {
            instructions.Add(new AssignmentInstruction(place, operand, span, scope));
        }

        //public void AddAction(Value value, TextSpan span, Scope scope)
        //{
        //    instructions.Add(new ActionStatement(value, span, scope));
        //}

        //public void AddDelete(IPlace place, UserObjectType type, TextSpan span, Scope scope)
        //{
        //    instructions.Add(new DeleteStatement(place, type, span, scope));
        //}

        //public void AddGoto(BlockBuilder exit, TextSpan span, Scope scope)
        //{
        //    instructions.Add(new GotoStatement(exit.BlockName, span, scope));
        //}

        //public void AddIf(IOperand condition, BlockBuilder thenBlock, BlockBuilder elseBlock, TextSpan span, Scope scope)
        //{
        //    instructions.Add(new IfStatement(condition, thenBlock.BlockName, elseBlock.BlockName, span, scope));
        //}

        //public void AddReturn(TextSpan span, Scope scope)
        //{
        //    instructions.Add(new ReturnStatement(span, scope));
        //}

        //public void AddExitScope(TextSpan span, Scope scope)
        //{
        //    instructions.Add(new ExitScopeStatement(span, scope));
        //}

        //public override string ToString()
        //{
        //    return BlockName.ToString();
        //}
    }
}
