using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class BasicBlock
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public readonly int Number; // The block number is used as its name in IR

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull, ItemNotNull] public FixedList<ExpressionStatement> ExpressionStatements { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull] public BlockTerminatorStatement Terminator { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        [NotNull, ItemNotNull] public FixedList<Statement> Statements { get; }

        public BasicBlock(int number,
            [NotNull, ItemNotNull] IEnumerable<ExpressionStatement> expressionStatements,
            [NotNull] BlockTerminatorStatement terminator)
        {
            Number = number;
            ExpressionStatements = expressionStatements.ToFixedList();
            Terminator = terminator;
            Statements = ExpressionStatements.Append<Statement>(terminator).NotNull().ToFixedList();
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay =>
            $"BB #{Number}, Statements={Statements.Count}";
    }
}
