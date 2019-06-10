using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
{
    /// <summary>
    /// The claims for each statement in a control flow graph
    /// </summary>
    public class ControlFlowGraphClaims
    {
        private readonly Dictionary<Statement, HashSet<Claim>> claims = new Dictionary<Statement, HashSet<Claim>>();

        public HashSet<Claim> After(Statement statement)
        {
            if (claims.TryGetValue(statement, out var existingClaims))
                return existingClaims;

            var newClaims = new HashSet<Claim>();
            claims.Add(statement, newClaims);
            return newClaims;
        }
    }
}
