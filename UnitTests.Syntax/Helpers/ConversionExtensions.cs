using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Helpers
{
    public static class ConversionExtensions
    {
        [NotNull]
        [ItemNotNull]
        public static string Concat([NotNull][ItemNotNull] this IEnumerable<PsuedoToken> tokens)
        {
            return string.Concat(tokens.Select(t => t.Text));
        }

        [NotNull]
        public static string Concat([NotNull][ItemNotNull] this IEnumerable<Token> tokens, [NotNull]  CodeFile file)
        {
            return string.Concat(tokens.Select(t => t.Text(file.Code)));
        }

        [NotNull]
        public static List<PsuedoToken> ToPsuedoTokens([NotNull] this IEnumerable<Token> tokens, [NotNull] CodeFile file)
        {
            return tokens.Select(t => PsuedoToken.For(t, file.Code)).ToList();
        }
    }
}
