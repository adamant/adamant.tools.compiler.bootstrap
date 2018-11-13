using System.Collections;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing
{
    [DebuggerDisplay("Entries = {values.Count}, Variables = {VariableCount}")]
    public class LiveVariables
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public int VariableCount { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        [NotNull, ItemNotNull]
        private readonly FixedList<FixedList<BitArray>> values;

        public LiveVariables([NotNull] ControlFlowGraph graph)
        {
            Requires.NotNull(nameof(graph), graph);
            VariableCount = graph.VariableDeclarations.Count;
            values = graph.BasicBlocks.Select(block =>
                block.Statements.Append<object>(block.Terminator).NotNull()
                    .Select(s => new BitArray(VariableCount)).ToFixedList()).ToFixedList();
        }

        //[NotNull]
        //public BitArray Before([NotNull] Statement statement)
        //{
        //    if (values.TryGetValue(statement, out var existingValue))
        //        return existingValue;

        //    var newValue = new BitArray(VariableCount);
        //    values.Add(statement, newValue);
        //    return newValue;
        //}

        /// <summary>
        /// Note, this will allow you to ask for the live variables before the
        /// block terminator as if it were a statement.
        /// </summary>
        [NotNull]
        public BitArray Before(int block, int statement)
        {
            return values[block][statement].NotNull();
        }
    }
}
