using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing
{
    public class Claims
    {
        private readonly Dictionary<StatementAnalysis, HashSet<Claim>> claims = new Dictionary<StatementAnalysis, HashSet<Claim>>();

        public HashSet<Claim> After(StatementAnalysis statement)
        {
            if (claims.TryGetValue(statement, out var existingClaims))
                return existingClaims;

            var newClaims = new HashSet<Claim>();
            claims.Add(statement, newClaims);
            return newClaims;
        }
    }
}
