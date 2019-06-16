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

        public void AddAssignment(Place place, Value value, TextSpan span, Scope scope)
        {
            statements.Add(new AssignmentStatement(place, value, span, scope));
        }

        public void AddAction(Value value, TextSpan span, Scope scope)
        {
            statements.Add(new ActionStatement(value, span, scope));
        }

        public void AddDelete(Place place, ObjectType type, TextSpan span, Scope scope)
        {
            statements.Add(new DeleteStatement(place, type, span, scope));
        }

        public void AddGoto(BlockBuilder exit, TextSpan span, Scope scope)
        {
            statements.Add(new GotoStatement(exit.BlockNumber, span, scope));
        }

        public void AddIf(Operand condition, BlockBuilder thenBlock, BlockBuilder elseBlock, TextSpan span, Scope scope)
        {
            statements.Add(new IfStatement(condition, thenBlock.BlockNumber, elseBlock.BlockNumber, span, scope));
        }

        public void AddReturn(TextSpan span, Scope scope)
        {
            statements.Add(new ReturnStatement(span, scope));
        }

        public void AddExitScope(TextSpan span, Scope scope)
        {
            statements.Add(new ExitScopeStatement(span, scope));
        }

        public override string ToString()
        {
            return $"bb{BlockNumber}";
        }
    }
}
