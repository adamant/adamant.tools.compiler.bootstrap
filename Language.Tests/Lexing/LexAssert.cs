using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public static class LexAssert
    {
        internal static void LexesCorrectly(TestTokenSequence sequence)
        {
            var source = new SourceText(sequence.ToString());
            var lexer = new Lexer(source);
            var tokens = lexer.ToList();
            Assert.Collection(tokens, sequence.WhereIsToken().Tokens.Select(Inspector).Append(AssertEndOfFile).ToArray());
        }
        private static Action<Token> Inspector(TestToken expected)
        {
            return token =>
            {
                Assert.Equal(expected.Kind.TokenKind, token.Kind);
                Assert.Equal(expected.Text, token.Text);
                if (expected.Value == null)
                    Assert.IsType<Token>(token);
                else if (expected.Value is string)
                    Assert.Equal(expected.Value, Assert.IsType<StringToken>(token).Value);
                else
                    throw new NotSupportedException($"Expected token value of `{expected.Value} not supported`");
                Assert.Equal(expected.IsValid, !token.DiagnosticInfos.Any(d => d.Level > DiagnosticLevel.Warning));
            };
        }
        private static void AssertEndOfFile(Token token)
        {
            Assert.Equal(TokenKind.EndOfFile, token.Kind);
            Assert.Equal(0, token.Span.Length);
        }
    }
}
