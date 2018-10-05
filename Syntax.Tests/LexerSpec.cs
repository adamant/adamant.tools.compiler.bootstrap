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

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tests
{
    public class LexerSpec
    {
        /// <summary>
        /// Registers generators needed for psuedo token generation. Invokes
        /// <see cref="PsuedoTokenGenerators.ArbitraryPsuedoToken"/>
        /// </summary>
        static LexerSpec()
        {
            Arb.Register<PsuedoTokenGenerators>();
        }

        private static Property Check(Func<bool> condition)
        {
            return condition.ToProperty();
        }

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
            Assert.Equal(TokenKind.Identifier, token.Kind);
            Assert.Equal(new TextSpan(0, identifier.Length), token.Span);
            Assert.Equal(value, token.Value);
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
            Assert.Equal(TokenKind.StringLiteral, token.Kind);
            Assert.Equal(new TextSpan(0, literal.Length), token.Span);
            Assert.Equal(value, token.Value);
        }

        [Theory]
        [MemberData(nameof(SymbolsTheoryData))]
        public void Symbols(string symbol, TokenKind kind)
        {
            var file = symbol.ToFakeCodeFile();
            var output = lexer.Lex(file);
            var token = AssertSingleTokenNoErrors(output);
            Assert.Equal(kind, token.Kind);
            Assert.Equal(new TextSpan(0, symbol.Length), token.Span);
            Assert.Null(token.Value);
        }

        //[Property]
        //public Property Valid_token_sequence_lexes_back_to_itself(NonNull<List<NonNull<PsuedoToken>>> tokens)
        //{
        //    var input = string.Concat(tokens.Get.Select(t => t.Get.Text));
        //    var fi = input.ToFakeCodeFile();
        //    var output = lexer.Lex(fi);
        //    return Check(() => tokens.Get.Select(t => t.Get).SequenceEqual(output.Select(t => PsuedoToken.For(t, fi.Code)))).Check(new Configuration() { });
        //}

        [Fact]
        public void End_of_file_in_block_comment()
        {
            var tokens = lexer.Lex("/*".ToFakeCodeFile()).ToList();
            Assert.Collection(tokens, comment =>
            {
                Assert.True(comment.Span.Length == 2, "Comment length of 2");
            }, t2 =>
            {
                var eof = (EndOfFileToken)t2;
                Assert.Collection(eof.Value, diagnostics => { });
            });
        }

        [Fact]
        public void End_of_file_in_string_gives_error()
        {
            var tokens = lexer.Lex(@"""".ToFakeCodeFile()).ToList();
            Assert.Collection(tokens, comment =>
            {
                Assert.True(comment.Span.Length == 2, "Comment length of 2");
            }, t2 =>
            {
                var eof = (EndOfFileToken)t2;
                Assert.Collection(eof.Value, diagnostics => { });
            });
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
        public bool Tokens_concatenate_to_input(NonNull<string> input)
        {
            var file = input.ToFakeCodeFile();
            var output = lexer.Lex(file);
            return input.Get == Concat(output, file);
        }

        [Property(MaxTest = 5_000)]
        public Property Token_lexes(NonNull<PsuedoToken> nonNullToken)
        {
            var token = nonNullToken.Get;
            var file = token.ToFakeCodeFile();
            var output = lexer.Lex(file);
            var outputAsPsuedoTokens = PsuedoTokensFor(output, file);
            var expectedPsuedoTokens = token.Yield().Append(PsuedoToken.EndOfFile());
            return Check(() => expectedPsuedoTokens.SequenceEqual(outputAsPsuedoTokens))
                .Label($"Output: {Display(outputAsPsuedoTokens)} != Expected: {Display(expectedPsuedoTokens)}");
        }

        #region Helper functions
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

        private static string Concat(IEnumerable<Token> tokens, CodeFile file)
        {
            return string.Concat(tokens.Select(t => t.Text(file.Code)));
        }

        private static List<PsuedoToken> PsuedoTokensFor(IEnumerable<Token> tokens, CodeFile file)
        {
            return tokens.Select(t => PsuedoToken.For(t, file.Code)).ToList();
        }

        private static string Display(IEnumerable<PsuedoToken> tokens)
        {
            return string.Join(", ", tokens);
        }

        public static IEnumerable<object[]> SymbolsTheoryData()
        {
            return PsuedoTokenGenerators.Symbols.Select(item => new object[] { item.Key, item.Value });
        }
        #endregion
    }
}
