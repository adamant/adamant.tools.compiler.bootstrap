using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Helpers
{
    public static class ConversionExtensions
    {
        [NotNull]
        public static string Concat([NotNull, ItemNotNull] this IEnumerable<PsuedoToken> tokens)
        {
            return string.Concat(tokens.Select(t => t.Text));
        }

        [NotNull]
        public static string Concat([NotNull, ItemNotNull] this IEnumerable<IToken> tokens, [NotNull]  CodeFile file)
        {
            return string.Concat(tokens.Select(t => t.Text(file.Code)));
        }

        [NotNull]
        public static List<PsuedoToken> ToPsuedoTokens([NotNull] this IEnumerable<IToken> tokens, [NotNull] CodeFile file)
        {
            return tokens.Select(t => PsuedoToken.For(t, file.Code)).ToList();
        }
    }
}
