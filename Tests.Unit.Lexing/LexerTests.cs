using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing.Fakes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing.Helpers;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using FsCheck;
using FsCheck.Xunit;
using Xunit;


namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Lexing
{
    [Trait("Category", "Lexing")]
    public class LexerTests
    {
        private static LexResult Lex(string text)
        {
            var lexer = new Lexer();
            var context = FakeParseContext.For(text);
            return new LexResult(lexer.Lex(context));
        }

        [Theory]
        [InlineData("hello", "hello")]
        [InlineData(@"\class", "class")]
        [InlineData(@"\""Hello World!""", "Hello World!", Skip = "Escaped String Identifiers Not Implemented")]
        public void Identifier_value(string identifier, string value)
        {
            var result = Lex(identifier);
            var token = result.AssertSingleToken();
            token.AssertIdentifier(0, identifier.Length, value);
            result.AssertNoDiagnostics();
        }

        [Theory]
        // Keywords Not Yet Implemented
        [InlineData("ensures")]
        [InlineData("implicit")]
        [InlineData("invariant")]
        [InlineData("params")]
        [InlineData("ref")]
        [InlineData("throw")]
        [InlineData("where")]
        // Actual Reserved Words
        [InlineData("alias")]
        [InlineData("partial")]
        [InlineData("xor")]
        public void Reserved_words(string reservedWord)
        {
            // TODO this error should only be reported at the declaration site, not the use site
            var result = Lex(reservedWord);
            var token = result.AssertSingleToken();
            token.AssertIdentifier(0, reservedWord.Length, reservedWord);
            var diagnostic = result.AssertSingleDiagnostic();
            diagnostic.AssertError(1006, 0, reservedWord.Length);
        }

        [Fact]
        public void Continue_as_next()
        {
            // TODO this error should be contextual so that it reports reserved word when used as a keyword, but this when used as `next`
            const string word = "continue";
            var result = Lex(word);
            var token = result.AssertSingleToken();
            token.AssertIdentifier(0, word.Length, word);
            var diagnostic = result.AssertSingleDiagnostic();
            diagnostic.AssertError(1007, 0, word.Length);
        }

        [Theory]
        // Keywords
        [InlineData("class")]
        [InlineData("int")]
        // Keywords not yet implemented
        [InlineData("invariant")]
        [InlineData("ensures")]
        [InlineData("params")]
        // Reserved words
        [InlineData("alias")]
        [InlineData("partial")]
        [InlineData("xor")]
        // Reserved type names
        [InlineData("int12")]
        [InlineData("uint96")]
        [InlineData("float42")]
        [InlineData("fixed")]
        [InlineData("fixed8.8", Skip = "Not implemented, doesn't lex as identifier")]
        [InlineData("ufixed8.24", Skip = "Not implemented, doesn't lex as identifier")]
        [InlineData("decimal")]
        [InlineData("decimal32")]
        [InlineData("real")]
        [InlineData("real.45", Skip = "Doesn't lex as identifier")]
        // Start with digits
        [InlineData("0")]
        [InlineData("1")]
        [InlineData("9")]
        [InlineData("42_answer")]
        public void Escaped_identifiers(string identifier)
        {
            var result = Lex("\\"+identifier);
            var token = result.AssertSingleToken();
            token.AssertIdentifier(0, identifier.Length+1, identifier);
            result.AssertNoDiagnostics();
        }

        [Theory]
        [InlineData("hello")]
        [InlineData("x")]
        [InlineData("foo")]
        public void Escaped_identifier_not_reserved(string identifier)
        {
            var result = Lex("\\" + identifier);
            var token = result.AssertSingleToken();
            token.AssertIdentifier(0, identifier.Length + 1, identifier);
            var diagnostic = result.AssertSingleDiagnostic();
            diagnostic.AssertError(1008, 0, identifier.Length + 1);
        }

        [Theory]
        [InlineData(@"""Hello World!""", "Hello World!")]
        [InlineData(@""" \r \n \0 \t \"" \' """, " \r \n \0 \t \" ' ")] // basic escape sequences
        [InlineData(@"""\u(2660)""", "\u2660")] // basic unicode escape sequences
        [InlineData(@""" \u(FFFF) \u(a) """, " \uFFFF \u000A ")]
        [InlineData(@""" \u(10FFFF) """, " \uDBFF\uDFFF ")] // Surrogate Pair
        public void String_literal_value(string literal, string value)
        {
            var result = Lex(literal);
            var token = result.AssertSingleToken();
            token.AssertStringLiteral(0, literal.Length, value);
            result.AssertNoDiagnostics();
        }

        [Theory]
        [MemberData(nameof(Symbols_lex_TheoryData))]
        public void Symbols_lex(string symbol, Type tokenType)
        {
            var result = Lex(symbol);
            var token = result.AssertSingleToken();
            Assert.OfType(tokenType, token);
            Assert.Equal(new TextSpan(0, symbol.Length), token.Span);
            result.AssertNoDiagnostics();
        }

        public static IEnumerable<object[]> Symbols_lex_TheoryData()
        {
            return Arbitrary.Symbols.Select(item => new object[] { item.Key, item.Value });
        }

        [Fact]
        public void Error_for_c_style_not_equals_operator()
        {
            var result = Lex("x!=y");
            result.AssertTokens(3);
            result.Tokens[0].AssertIdentifier(0, 1, "x");
            result.Tokens[1].AssertIs<INotEqualToken>(1, 2);
            result.Tokens[2].AssertIdentifier(3, 1, "y");
            var diagnostic = result.AssertSingleDiagnostic();
            diagnostic.AssertError(1004, 1, 2);
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
            var result = Lex(comment);
            var token = result.AssertSingleToken();
            token.AssertIs<ICommentToken>(0, comment.Length);
            result.AssertNoDiagnostics();
        }

        [Theory]
        [InlineData(@"/*")]
        [InlineData(@"/*/")]
        public void End_of_file_in_block_comment(string comment)
        {
            var result = Lex(comment);
            var token = result.AssertSingleToken();
            token.AssertIs<ICommentToken>(0, comment.Length);
            var diagnostic = result.AssertSingleDiagnostic();
            diagnostic.AssertError(1001, 0, comment.Length);
        }

        [Fact]
        public void End_of_file_in_line_comment()
        {
            var result = Lex("// hello");
            var token = result.AssertSingleToken();
            token.AssertIs<ICommentToken>(0, 8);
            result.AssertNoDiagnostics();
        }

        [Fact]
        public void End_of_file_in_string()
        {
            var result = Lex(@"""Hello");
            var token = result.AssertSingleToken();
            token.AssertStringLiteral(0, 6, "Hello");
            var diagnostic = result.AssertSingleDiagnostic();
            diagnostic.AssertError(1002, 0, 6);
        }

        [Theory]
        [InlineData(@"""\", "")]
        [InlineData(@"""\a""", "a")]
        [InlineData(@"""\m""", "m")]
        [InlineData(@"""\x""", "x")]
        // Unicode escapes could be stopped with EOF, End of string or invalid char
        // at each of the different states
        [InlineData(@"""\u""", "u")]
        [InlineData(@"""\u", "u")]
        [InlineData(@"""\u|""", "u|")]
        [InlineData(@"""\u(", "u(")]
        [InlineData(@"""\u(""", "u(")]
        [InlineData(@"""\u(|""", "u(|")]
        [InlineData(@"""\u(1f", "\u001f")]
        [InlineData(@"""\u(1f""", "\u001f")]
        [InlineData(@"""\u(1f|""", "\u001f|")]
        [InlineData(@"""\u()""", "u()")]
        [InlineData(@"""\u(110000)""", "u(110000)")] // 1 too high
        // TODO can't put use a surrogate pair as a unicode escape, they must be unicode scalars
        public void Invalid_escape_sequence(string literal, string expectedValue)
        {
            var result = Lex(literal);
            var token = result.AssertSingleToken();
            token.AssertStringLiteral(0, literal.Length, expectedValue);
            var completeString = literal.EndsWith("\"", StringComparison.Ordinal);
            var expectedDiagnosticCount = completeString ? 1 : 2;
            result.AssertDiagnostics(expectedDiagnosticCount);
            var diagnostics = result.Diagnostics;

            var expectedLength = literal.Length;
            if (literal.Contains('|', StringComparison.Ordinal)) expectedLength -= 1;
            if (completeString)
                diagnostics[0].AssertError(1003, 1, expectedLength - 2);
            else
            {
                diagnostics[0].AssertError(1002, 0, literal.Length);
                diagnostics[1].AssertError(1003, 1, expectedLength - 1);
            }
        }

        [Theory]
        [InlineData(" % ")]
        [InlineData(" ~ ")]
        [InlineData(" ◊ ")]
        [InlineData(" \u0007 ")] // Bell Character
        public void Unexpected_character(string text)
        {
            var result = Lex(text);
            result.AssertTokens(3);
            result.Tokens[0].AssertIs<IWhitespaceToken>(0, 1);
            result.Tokens[1].AssertIs<IUnexpectedToken>(1, 1);
            result.Tokens[2].AssertIs<IWhitespaceToken>(2, 1);
            var diagnostic = result.AssertSingleDiagnostic();
            diagnostic.AssertError(1005, 1, 1);
            Assert.True(diagnostic.Message.Contains(text[1], StringComparison.Ordinal), "Doesn't contain character");
        }

        [Theory]
        // Not implemented
        [InlineData("[")]
        [InlineData("]")]
        [InlineData("#")]
        [InlineData("#(")]
        [InlineData("#[")]
        [InlineData("#{")]
        [InlineData("|")]
        [InlineData("&")]
        [InlineData("@")]
        [InlineData("^")]
        [InlineData("^.")]
        // Reserved
        [InlineData("$")]
        [InlineData("`")]
        [InlineData("**")]
        [InlineData("##")]
        public void Reserved_operator(string text)
        {
            var result = Lex(text);
            var token = result.AssertSingleToken();
            token.AssertIs<IUnexpectedToken>(0, text.Length);
            var diagnostic = result.AssertSingleDiagnostic();
            diagnostic.AssertError(1009, 0, text.Length);
        }

        [Property(MaxTest = 10_000)]
        public Property Tokens_concatenate_to_input()
        {
            return Prop.ForAll<NonNull<string>>(input =>
            {
                var inputString = input.NotNull(nameof(input));
                var result = Lex(inputString);
                return inputString == result.TokensToString();
            });
        }

        [Property(MaxTest = 1_000)]
        public Property Token_lexes()
        {
            return Prop.ForAll(Arbitrary.PsuedoToken(), token =>
            {
                var result = Lex(token.Text);
                var outputAsPsuedoTokens = result.ToPsuedoTokens();
                var expectedPsuedoTokens = token.Yield().Append(PsuedoToken.EndOfFile()).ToList();
                return expectedPsuedoTokens.SequenceEqual(outputAsPsuedoTokens)
                    .Label($"Actual:   {outputAsPsuedoTokens.DebugFormat()}")
                    .Label($"Expected: {expectedPsuedoTokens.DebugFormat()}")
                    .Collect(token.TokenType.Name);
            });
        }

        [Property(MaxTest = 200)]
        public Property Valid_token_sequence_lexes_back_to_itself()
        {
            return Prop.ForAll(Arbitrary.PsuedoTokenList(), tokens =>
            {
                var input = string.Concat(tokens.Select(t => t.Text));
                var result = Lex(input);
                var outputAsPsuedoTokens = result.ToPsuedoTokens();
                var expectedPsuedoTokens = tokens.Append(PsuedoToken.EndOfFile()).ToFixedList();
                return expectedPsuedoTokens.SequenceEqual(outputAsPsuedoTokens)
                    .Label($"Text: „{input.Escape()}„")
                    .Label($"Actual:   {outputAsPsuedoTokens.DebugFormat()}")
                    .Label($"Expected: {expectedPsuedoTokens.DebugFormat()}");
            });
        }
    }
}
