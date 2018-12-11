using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class BasicBlock
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public readonly int Number; // The block number is used as its name in IR

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public FixedList<ExpressionStatement> ExpressionStatements { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public BlockTerminatorStatement Terminator { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public FixedList<Statement> Statements { get; }

        public BasicBlock(int number,
            IEnumerable<ExpressionStatement> expressionStatements,
            BlockTerminatorStatement terminator)
        {
            Number = number;
            ExpressionStatements = expressionStatements.ToFixedList();
            Terminator = terminator;
            Statements = ExpressionStatements.Append<Statement>(terminator).ToFixedList();
            foreach (var (statement, i) in Statements.Enumerate())
            {
                statement.BlockNumber = number;
                statement.Number = i;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay =>
            $"BB #{Number}, Statements={Statements.Count}";
    }
}
