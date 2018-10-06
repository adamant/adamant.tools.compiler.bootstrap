using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core.Tests;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests
{
    public class LexerSpec
    {
        private readonly Lexer lexer = new Lexer();

        [Theory]
        [InlineData("hello", "hello")]
        [InlineData(@"\class", "class", Skip = "Escaped Identifiers Not Implemented")]
        [InlineData(@"\""Hello World!""", "Hello World!", Skip = "Escaped String Identifiers Not Implemented")]
        public void Identifier_value(string identifier, string value)
        {
            var file = identifier.ToFakeCodeFile();
            var output = lexer.Lex(file);
            var token = AssertSingleTokenNoErrors(output);
            AssertToken(token, TokenKind.Identifier, 0, identifier.Length, value);
        }

        [Theory]
        [InlineData(@"""Hello World!""", "Hello World!")]
        [InlineData(@""" \r \n \0 \t \"" \' """, " \r \n \0 \t \" ' ")]
        [InlineData(@"""\u(2660)""", "\u2660")]
        [InlineData(@""" \u(FFFF) \u(a) """, " \uFFFF \u000A ")]
        [InlineData(@""" \u(10FFFF) """, " \uDBFF\uDFFF ")] // Surrogate Pair
        public void String_literal_value(string literal, string value)
        {
            var file = literal.ToFakeCodeFile();
            var output = lexer.Lex(file);
            var token = AssertSingleTokenNoErrors(output);
            AssertToken(token, TokenKind.StringLiteral, 0, literal.Length, value);
        }

        [Theory]
        [MemberData(nameof(SymbolsTheoryData))]
        public void Symbols(string symbol, TokenKind kind)
        {
            var file = symbol.ToFakeCodeFile();
            var output = lexer.Lex(file);
            var token = AssertSingleTokenNoErrors(output);
            AssertToken(token, kind, 0, symbol.Length);
        }

        [Theory]
        [InlineData(@"/*c*/")]
        [InlineData(@"/**/")]
        [InlineData(@"/***/")]
        [InlineData(@"/*/**/")]
        [InlineData(@"/* *c */")]
        [InlineData(@"//c")]
        [InlineData(@"//\r")]
        [InlineData(@"// foo")]
        public void Comments(string comment)
        {
            var file = comment.ToFakeCodeFile();
            var output = lexer.Lex(file);
            var token = AssertSingleTokenNoErrors(output);
            AssertToken(token, TokenKind.Comment, 0, comment.Length);
        }

        [Fact]
        public void End_of_file_in_block_comment()
        {
            var file = "/*".ToFakeCodeFile();
            var tokens = lexer.Lex(file).ToList();
            var (comment, diagnostics) = AssertSingleTokenWithErrors(tokens);
            AssertToken(comment, TokenKind.Comment, 0, 2);
            var diagnostic = AssertSingleDiagnostic(diagnostics);
            AssertError(diagnostic, 1001, 0, 2);
        }

        [Fact]
        public void End_of_file_in_string_gives_error()
        {
            var tokens = lexer.Lex(@"""Hello".ToFakeCodeFile()).ToList();
            var (literal, diagnostics) = AssertSingleTokenWithErrors(tokens);
            AssertToken(literal, TokenKind.StringLiteral, 0, 6, "Hello");
            var diagnostic = AssertSingleDiagnostic(diagnostics);
            AssertError(diagnostic, 1002, 0, 6);
        }

        [Fact]
        public void End_of_file_unicode_escape()
        {
            var tokens = lexer.Lex(@"""\u".ToFakeCodeFile()).ToList();
            Assert.Collection(tokens, comment =>
            {
                Assert.True(comment.Span.Length == 2, "Comment length of 2");
            }, t2 =>
            {
                var eof = (EndOfFileToken)t2;
                Assert.Collection(eof.Value, diagnostics => { });
            });
        }

        [Property(MaxTest = 10_000)]
        public Property Tokens_concatenate_to_input()
        {
            return Prop.ForAll<NonNull<string>>(input =>
            {
                var file = input.ToFakeCodeFile();
                var output = lexer.Lex(file);
                return input.Get == Concat(output, file);
            });
        }

        [Property(MaxTest = 1_000)]
        public Property Token_lexes()
        {
            return Prop.ForAll(Arbitrary.PsuedoToken(), token =>
            {
                var file = token.ToFakeCodeFile();
                var output = lexer.Lex(file);
                var outputAsPsuedoTokens = PsuedoTokensFor(output, file);
                var expectedPsuedoTokens = token.Yield().Append(PsuedoToken.EndOfFile()).ToList();
                return expectedPsuedoTokens.SequenceEqual(outputAsPsuedoTokens)
                    .Label($"Output: {Display(outputAsPsuedoTokens)} != Expected: {Display(expectedPsuedoTokens)}")
                    .Collect(token.Kind);
            });
        }

        [Property]
        public Property Valid_token_sequence_lexes_back_to_itself()
        {
            return Prop.ForAll(Arbitrary.PsuedoTokenList(), tokens =>
            {
                var input = Concat(tokens);
                var file = input.ToFakeCodeFile();
                var output = lexer.Lex(file);
                var outputAsPsuedoTokens = PsuedoTokensFor(output, file);
                var expectedPsuedoTokens = tokens.Append(PsuedoToken.EndOfFile()).ToList();
                return expectedPsuedoTokens.SequenceEqual(outputAsPsuedoTokens)
                    .Label($"Output: {Display(outputAsPsuedoTokens)} != Expected: {Display(expectedPsuedoTokens)}");
            });
        }

        #region Helper functions
        private static void AssertToken(
            Token actual,
            TokenKind expectedKind,
            int expectedStart,
            int expectedLength,
            object expectedValue = null)
        {
            Assert.Equal(expectedKind, actual.Kind);
            Assert.True(expectedStart == actual.Span.Start, $"Expected token start {expectedStart}, was {actual.Span.Start}");
            Assert.True(expectedLength == actual.Span.Length, $"Expected token length {expectedLength}, was {actual.Span.Length}");
            Assert.Equal(expectedValue, actual.Value);
        }

        private static Token AssertSingleTokenNoErrors(IEnumerable<Token> tokens)
        {
            var list = tokens.ToList();
            Assert.Collection(list, token => { },
                eof =>
                {
                    Assert.Equal(TokenKind.EndOfFile, eof.Kind);
                    Assert.Equal(new TextSpan(list[0].Span.End, 0), eof.Span);
                    Assert.Empty((IReadOnlyList<Diagnostic>)eof.Value);
                });
            return list[0];
        }
        private static (Token, IReadOnlyList<Diagnostic>) AssertSingleTokenWithErrors(IEnumerable<Token> tokens)
        {
            var list = tokens.ToList();
            Assert.Collection(list, token => { },
                    eof =>
                    {
                        Assert.Equal(TokenKind.EndOfFile, eof.Kind);
                        Assert.Equal(new TextSpan(list[0].Span.End, 0), eof.Span);
                        Assert.NotEmpty((IReadOnlyList<Diagnostic>)eof.Value);
                    });
            return (list[0], (IReadOnlyList<Diagnostic>)list[1].Value);
        }

        private static Diagnostic AssertSingleDiagnostic(IReadOnlyList<Diagnostic> diagnostics)
        {
            Assert.True(diagnostics.Count == 1, $"Expected single diagnostic, were {diagnostics.Count}");
            return diagnostics[0];
        }

        private static void AssertError(Diagnostic diagnostic, int errorCode, int start, int length)
        {
            Assert.Equal(DiagnosticLevel.CompilationError, diagnostic.Level);
            AssertDiagnostic(diagnostic, errorCode, start, length);
        }
        private static void AssertDiagnostic(Diagnostic diagnostic, int errorCode, int start, int length)
        {
            Assert.Equal(DiagnosticPhase.Lexing, diagnostic.Phase);
            Assert.Equal(errorCode, diagnostic.ErrorCode);
            Assert.True(start == diagnostic.Span.Start, $"Expected diagnostic start {start}, was {diagnostic.Span.Start}");
            Assert.True(length == diagnostic.Span.Length, $"Expected diagnostic length {length}, was {diagnostic.Span.Length}");
        }

        private static string Concat(IEnumerable<PsuedoToken> tokens)
        {
            return string.Concat(tokens.Select(t => t.Text));
        }
        private static string Concat(IEnumerable<Token> tokens, CodeFile file)
        {
            return string.Concat(tokens.Select(t => t.Text(file.Code)));
        }

        public static List<PsuedoToken> PsuedoTokensFor(IEnumerable<Token> tokens, CodeFile file)
        {
            return tokens.Select(t => PsuedoToken.For(t, file.Code)).ToList();
        }

        private static string Display(IEnumerable<PsuedoToken> tokens)
        {
            return string.Join(", ", tokens);
        }

        public static IEnumerable<object[]> SymbolsTheoryData()
        {
            return Arbitrary.Symbols.Select(item => new object[] { item.Key, item.Value });
        }
        #endregion
    }
}
