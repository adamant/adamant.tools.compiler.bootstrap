using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing
{
    public class Claims
    {
        [NotNull]
        private readonly Dictionary<ExpressionStatement, HashSet<Claim>> claims = new Dictionary<ExpressionStatement, HashSet<Claim>>();

        [NotNull]
        public HashSet<Claim> After([NotNull] ExpressionStatement statement)
        {
            if (claims.TryGetValue(statement, out var existingClaims))
                return existingClaims;

            var newClaims = new HashSet<Claim>();
            claims.Add(statement, newClaims);
            return newClaims;
        }
    }
}
