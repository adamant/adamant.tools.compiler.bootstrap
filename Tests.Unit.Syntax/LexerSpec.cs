using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Fakes;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers;
using Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax.Helpers;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using FsCheck;
using FsCheck.Xunit;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Syntax
{
    [UnitTest]
    [Category("Lex")]
    public class LexerSpec
    {
        [NotNull]
        private readonly Lexer lexer = new Lexer();

        [Theory]
        [InlineData("hello", "hello")]
        [InlineData(@"\class", "class")]
        // TODO [InlineData(@"\""Hello World!""", "Hello World!", Skip = "Escaped String Identifiers Not Implemented")]
        public void Identifier_value([NotNull] string identifier, [NotNull] string value)
        {
            var file = identifier.ToFakeCodeFile();
            var output = lexer.Lex(file);
            var token = output.AssertSingleNoErrors();
            token.AssertIdentifier(0, identifier.Length, value);
        }

        [Theory]
        [InlineData(@"""Hello World!""", "Hello World!")]
        [InlineData(@""" \r \n \0 \t \"" \' """, " \r \n \0 \t \" ' ")] // basic escape sequences
        [InlineData(@"""\u(2660)""", "\u2660")] // basic unicode escape sequences
        [InlineData(@""" \u(FFFF) \u(a) """, " \uFFFF \u000A ")]
        [InlineData(@""" \u(10FFFF) """, " \uDBFF\uDFFF ")] // Surrogate Pair
        public void String_literal_value([NotNull] string literal, [NotNull] string value)
        {
            var file = literal.ToFakeCodeFile();
            var output = lexer.Lex(file);
            var token = output.AssertSingleNoErrors();
            token.AssertStringLiteral(0, literal.Length, value);
        }

        [Theory]
        [MemberData(nameof(SymbolsTheoryData))]
        public void Symbols([NotNull] string symbol, [NotNull] Type tokenType)
        {
            var file = symbol.ToFakeCodeFile();
            var output = lexer.Lex(file);
            var token = output.AssertSingleNoErrors();

            token.AssertOfType(tokenType);
            Assert.Equal(new TextSpan(0, symbol.Length), token.Span);
        }

        public static IEnumerable<object[]> SymbolsTheoryData()
        {
            return Arbitrary.Symbols.Select(item => new object[] { item.Key, item.Value });
        }

        [Fact]
        public void Error_for_c_style_not_equals_operator()
        {
            var file = "x!=y".ToFakeCodeFile();
            var output = lexer.Lex(file);
            var (token, diagnostics) = output.AssertCount(3);
            token[0].AssertIdentifier(0, 1, "x");
            token[1].AssertIs<INotEqualToken>(1, 2);
            token[2].AssertIdentifier(3, 1, "y");
            diagnostics.AssertCount(1);
            diagnostics[0].AssertError(1004, 1, 2);
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
        public void Comments([NotNull] string comment)
        {
            var file = comment.ToFakeCodeFile();
            var output = lexer.Lex(file);
            var token = output.AssertSingleNoErrors();
            token.AssertIs<ICommentToken>(0, comment.Length);
        }

        [Fact]
        public void End_of_file_in_block_comment()
        {
            var file = "/*".ToFakeCodeFile();
            var tokens = lexer.Lex(file);
            var (comment, diagnostics) = tokens.AssertSingleWithErrors();
            comment.AssertIs<ICommentToken>(0, 2);
            var diagnostic = diagnostics.AssertSingle();
            diagnostic.AssertError(1001, 0, 2);
        }

        // TODO End of file on line comment
        // TODO `/*/` ?

        [Fact]
        public void End_of_file_in_string()
        {
            var file = @"""Hello".ToFakeCodeFile();
            var tokens = lexer.Lex(file);
            var (token, diagnostics) = tokens.AssertSingleWithErrors();
            token.AssertStringLiteral(0, 6, "Hello");
            var diagnostic = diagnostics.AssertSingle();
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
        public void Invalid_escape_sequence([NotNull] string literal, [NotNull] string expectedValue)
        {
            var file = literal.ToFakeCodeFile();
            var tokens = lexer.Lex(file);
            var (token, diagnostics) = tokens.AssertSingleWithErrors();
            token.AssertStringLiteral(0, literal.Length, expectedValue);
            var completeString = literal.EndsWith("\"");
            var expectedDiagnosticCount = completeString ? 1 : 2;
            var diagnostic = diagnostics.AssertCount(expectedDiagnosticCount);

            var expectedLength = literal.Length;
            if (literal.Contains('|')) expectedLength -= 1;
            if (completeString)
                diagnostic[0].AssertError(1003, 1, expectedLength - 2);
            else
            {
                diagnostic[0].AssertError(1002, 0, literal.Length);
                diagnostic[1].AssertError(1003, 1, expectedLength - 1);
            }
        }

        [Theory]
        [InlineData(" % ")]
        [InlineData(" ~ ")]
        [InlineData(" ` ")]
        [InlineData(" \u0007 ")] // Bell Character
        public void Unexpected_character([NotNull] string text)
        {
            var file = text.ToFakeCodeFile();
            var output = lexer.Lex(file);
            var (token, diagnostics) = output.AssertCount(3);
            token[0].AssertIs<IWhitespaceToken>(0, 1);
            token[1].AssertIs<IUnexpectedToken>(1, 1);
            token[2].AssertIs<IWhitespaceToken>(2, 1);
            diagnostics.AssertCount(1);
            diagnostics[0].AssertError(1005, 1, 1);
            Assert.True(diagnostics[0]?.Message.Contains(text[1]), "Doesn't contain character");
        }

        [Property(MaxTest = 10_000)]
        public Property Tokens_concatenate_to_input()
        {
            return Prop.ForAll<NonNull<string>>(input =>
            {
                var file = input.ToFakeCodeFile();
                var output = lexer.Lex(file);
                return input.Get == output.Concat(file);
            });
        }

        [Property(MaxTest = 1_000)]
        public Property Token_lexes()
        {
            return Prop.ForAll(Arbitrary.PsuedoToken(), token =>
            {
                Assert.NotNull(token);
                var file = token.ToFakeCodeFile();
                var output = lexer.Lex(file);
                var outputAsPsuedoTokens = output.ToPsuedoTokens(file);
                var expectedPsuedoTokens = token.Yield().Append(PsuedoToken.EndOfFile()).AssertNotNull().ToList();
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
                var input = tokens.Concat();
                var file = input.ToFakeCodeFile();
                var output = lexer.Lex(file);
                var outputAsPsuedoTokens = output.ToPsuedoTokens(file);
                var expectedPsuedoTokens = tokens.Append(PsuedoToken.EndOfFile()).ToList();
                return expectedPsuedoTokens.SequenceEqual(outputAsPsuedoTokens)
                    .Label($"Text: „{file.Code.Text.Escape()}„")
                    .Label($"Actual:   {outputAsPsuedoTokens.DebugFormat()}")
                    .Label($"Expected: {expectedPsuedoTokens.DebugFormat()}");
            });
        }
    }
}
