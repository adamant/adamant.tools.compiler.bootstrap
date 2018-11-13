using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        [NotNull] private readonly Dictionary<Statement, BitArray> values = new Dictionary<Statement, BitArray>();

        public LiveVariables([NotNull] ControlFlowGraph graph)
        {
            Requires.NotNull(nameof(graph), graph);
            VariableCount = graph.VariableDeclarations.Count;
        }

        [NotNull]
        public BitArray Before([NotNull] Statement statement)
        {
            if (values.TryGetValue(statement, out var existingValue))
                return existingValue;

            var newValue = new BitArray(VariableCount);
            values.Add(statement, newValue);
            return newValue;
        }
    }
}
