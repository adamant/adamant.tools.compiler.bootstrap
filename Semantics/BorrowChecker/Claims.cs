using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.BorrowChecker
{
    public class Claims
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
