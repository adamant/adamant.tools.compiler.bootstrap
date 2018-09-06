using System.Collections;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements;
using Adamant.Tools.Compiler.Bootstrap.IL.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.BorrowChecker
{
    public class LiveVariables
    {
        public int VariableCount { get; }
        private readonly Dictionary<Statement, BitArray> values = new Dictionary<Statement, BitArray>();

        public LiveVariables(FunctionDeclaration function)
        {
            VariableCount = function.VariableDeclarations.Count;
        }

        public BitArray Before(Statement statement)
        {
            if (values.TryGetValue(statement, out var existingValue))
                return existingValue;

            var newValue = new BitArray(VariableCount);
            values.Add(statement, newValue);
            return newValue;
        }
    }
}
