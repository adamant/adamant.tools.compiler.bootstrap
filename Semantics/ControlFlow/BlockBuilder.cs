using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow
{
    public class BlockBuilder
    {
        public readonly int BlockNumber;
        private readonly List<Statement> statements = new List<Statement>();
        public IReadOnlyList<Statement> Statements => statements;
        public bool IsTerminated => statements.LastOrDefault() is BlockTerminatorStatement;
        public BlockTerminatorStatement Terminator => (BlockTerminatorStatement)statements.LastOrDefault();

        public BlockBuilder(int blockNumber)
        {
            BlockNumber = blockNumber;
        }

        public void AddAssignment(Place place, Value value)
        {
            statements.Add(new AssignmentStatement(place, value));
        }

        public void AddAction(Value value)
        {
            statements.Add(new ActionStatement(value));
        }

        public void AddDelete(VariableReference variable, TextSpan span)
        {
            statements.Add(new DeleteStatement(variable.VariableNumber, span));
        }

        public void AddGoto(BlockBuilder exit)
        {
            statements.Add(new GotoStatement(exit.BlockNumber));
        }

        public void AddIf(Operand condition, BlockBuilder thenBlock, BlockBuilder elseBlock)
        {
            statements.Add(new IfStatement(condition, thenBlock.BlockNumber, elseBlock.BlockNumber));
        }

        public void AddReturn()
        {
            statements.Add(new ReturnStatement());
        }

        public override string ToString()
        {
            return $"bb{BlockNumber}";
        }
    }
}
