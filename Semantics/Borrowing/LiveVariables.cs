using System.Collections;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing
{
    [DebuggerDisplay("Entries = {values.Count}, Variables = {VariableCount}")]
    public class LiveVariables
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int VariableCount { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        private readonly FixedList<FixedList<BitArray>> values;

        public LiveVariables(ControlFlowGraph graph)
        {
            VariableCount = graph.VariableDeclarations.Count;
            values = graph.BasicBlocks.Select(block =>
                block.ExpressionStatements.Append<object>(block.Terminator)
                    .Select(s => new BitArray(VariableCount)).ToFixedList()).ToFixedList();
        }

        /// <summary>
        /// Note, this will allow you to ask for the live variables before the
        /// block terminator as if it were a statement.
        /// </summary>
        public BitArray Before(Statement statement)
        {
            return values[statement.BlockNumber][statement.Number];
        }
    }
}
