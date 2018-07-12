using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using System;
using System.Linq;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public static class LexAssert
    {
        internal static void LexesTo(string text, TestToken[] expectedTokens)
        {
            var source = new SourceText(text);
            var lexer = new Lexer(source);
            var tokens = lexer.ToList();
            Assert.Collection(tokens, expectedTokens.Select(Inspector).Append(AssertEndOfFile).ToArray());
        }
        private static Action<Token> Inspector(TestToken expected)
        {
            return token =>
            {
                Assert.Equal(expected.Kind.TokenKind, token.Kind);
                Assert.Equal(expected.IsValid, !token.DiagnosticInfos.Any(d => d.Level > DiagnosticLevel.Warning));
                Assert.Equal(expected.Text, token.Text);
            };
        }
        private static void AssertEndOfFile(Token token)
        {
            Assert.Equal(TokenKind.EndOfFile, token.Kind);
            Assert.Equal(0, token.Span.Length);
        }
    }
}
