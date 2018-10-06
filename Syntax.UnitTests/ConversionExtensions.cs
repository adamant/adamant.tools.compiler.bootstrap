using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests
{
    public static class ConversionExtensions
    {
        public static string Concat(this IEnumerable<PsuedoToken> tokens)
        {
            return string.Concat(tokens.Select(t => t.Text));
        }

        public static string Concat(this IEnumerable<Token> tokens, CodeFile file)
        {
            return string.Concat(tokens.Select(t => t.Text(file.Code)));
        }

        public static List<PsuedoToken> ToPsuedoTokens(this IEnumerable<Token> tokens, CodeFile file)
        {
            return tokens.Select(t => PsuedoToken.For(t, file.Code)).ToList();
        }
    }
}
