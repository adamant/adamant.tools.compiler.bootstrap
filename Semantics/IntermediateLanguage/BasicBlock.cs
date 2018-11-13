using System.Collections.Generic;
using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class BasicBlock
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public readonly int Number; // The block number is used as its name in IR
        [NotNull, ItemNotNull] public FixedList<Statement> Statements { get; }
        [NotNull] public BlockTerminator Terminator { get; }

        public BasicBlock(int number,
            [NotNull, ItemNotNull] IEnumerable<Statement> statements,
            [NotNull] BlockTerminator terminator)
        {
            Number = number;
            Statements = statements.ToFixedList();
            Terminator = terminator;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay =>
            $"BB #{Number}, Statements={Statements.Count}";
    }
}
