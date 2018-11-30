using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing
{
    public class Claims
    {
        [NotNull]
        private readonly Dictionary<Statement, HashSet<Claim>> claims = new Dictionary<Statement, HashSet<Claim>>();

        [NotNull]
        public HashSet<Claim> After([NotNull] Statement statement)
        {
            if (claims.TryGetValue(statement, out var existingClaims))
                return existingClaims;

            var newClaims = new HashSet<Claim>();
            claims.Add(statement, newClaims);
            return newClaims;
        }
    }
}
