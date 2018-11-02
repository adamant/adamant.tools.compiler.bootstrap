using System.Collections;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Statements;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing
{
    public class LiveVariables
    {
        public int VariableCount { get; }
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
