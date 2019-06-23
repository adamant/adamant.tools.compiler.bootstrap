using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    /// <summary>
    /// The claims for each statement in a control flow graph
    /// </summary>
    public class StatementClaims
    {
        public readonly Claims ParameterClaims;
        private readonly Dictionary<Statement, Claims> claimsAfter = new Dictionary<Statement, Claims>();

        public StatementClaims(Claims parameterClaims)
        {
            ParameterClaims = parameterClaims;
        }

        public Claims After(Statement statement)
        {
            if (claimsAfter.TryGetValue(statement, out var existingClaims))
                return existingClaims;

            var newClaims = new Claims();
            claimsAfter.Add(statement, newClaims);
            return newClaims;
        }
    }
}
