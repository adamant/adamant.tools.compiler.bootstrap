using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    /// <summary>
    /// The claims for each statement in a control flow graph
    /// </summary>
    public class ControlFlowGraphClaims
    {
        public readonly Claims ParameterClaims;
        private readonly Dictionary<Statement, Claims> claims = new Dictionary<Statement, Claims>();

        public ControlFlowGraphClaims(Claims parameterClaims)
        {
            ParameterClaims = parameterClaims;
        }

        public Claims After(Statement statement)
        {
            if (claims.TryGetValue(statement, out var existingClaims))
                return existingClaims;

            var newClaims = new Claims();
            claims.Add(statement, newClaims);
            return newClaims;
        }
    }
}
