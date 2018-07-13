using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public static class LexAssert
    {
        public static void LexesCorrectly(TestToken token)
        {
            LexesCorrectly(TestTokenSequence.Single(token));
        }

        public static void LexesCorrectly(TestTokenSequence sequence)
        {
            var source = new SourceText(sequence.ToString());
            var lexer = new Lexer();
            var tokens = lexer.Lex(source);
            Assert.Collection(tokens, sequence.WhereIsToken().Tokens.Select(Inspector).Append(AssertEndOfFile).ToArray());
        }
        private static Action<Token> Inspector(TestToken expected)
        {
            return token => AssertMatch(expected, token);
        }

        private static void AssertMatch(TestToken expected, Token token)
        {
            Assert.Equal(expected.Kind.TokenKind, token.Kind);
            Assert.Equal(expected.Text, token.Text);
            switch (token.Kind)
            {
                case TokenKind.Identifier:
                    var identifierToken = Assert.IsType<IdentifierToken>(token);
                    if (expected.Value != null)
                        Assert.Equal(expected.Value, identifierToken.Value);
                    break;
                case TokenKind.StringLiteral:
                    var stringLiteralToken = Assert.IsType<StringToken>(token);
                    if (expected.Value != null)
                        Assert.Equal(expected.Value, stringLiteralToken.Value);
                    break;
                default:
                    Assert.IsType<Token>(token);
                    Assert.Null(expected.Value);
                    break;
            }
            Assert.Equal(expected.IsValid, !token.DiagnosticInfos.Any(d => d.Level > DiagnosticLevel.Warning));
        }

        private static void AssertEndOfFile(Token token)
        {
            Assert.Equal(TokenKind.EndOfFile, token.Kind);
            Assert.Equal(0, token.Span.Length);
        }
    }
}
