using System.Collections;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow.Graph;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing
{
    public class LiveVariables
    {
        public int VariableCount { get; }
        private readonly Dictionary<StatementAnalysis, BitArray> values = new Dictionary<StatementAnalysis, BitArray>();

        public LiveVariables(ControlFlowGraph function)
        {
            VariableCount = function.VariableDeclarations.Count;
        }

        public BitArray Before(StatementAnalysis statement)
        {
            if (values.TryGetValue(statement, out var existingValue))
                return existingValue;

            var newValue = new BitArray(VariableCount);
            values.Add(statement, newValue);
            return newValue;
        }
    }
}
