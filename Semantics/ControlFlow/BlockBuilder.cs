using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

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

        public void Add(Statement statement)
        {
            // We have to clone the statement because the statement number will
            // be assigned into it. Each block needs to have separate statements.
            statements.Add(statement.Clone());
        }

        public void AddAssignment(Place place, Value value, TextSpan span)
        {
            statements.Add(new AssignmentStatement(place, value, span));
        }

        public void AddAction(Value value, TextSpan span)
        {
            statements.Add(new ActionStatement(value, span));
        }

        public void AddDelete(Place place, ObjectType type, TextSpan span)
        {
            statements.Add(new DeleteStatement(place, type, span));
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

        public void AddEndScope(Place place, TextSpan span)
        {
            statements.Add(new EndScopeStatement(place, span));
        }

        public override string ToString()
        {
            return $"bb{BlockNumber}";
        }
    }
}
