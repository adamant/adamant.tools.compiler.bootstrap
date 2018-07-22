using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests
{
    public class LexerTests
    {
        private readonly ITestOutputHelper output;

        public LexerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [Category("Lexer")]
        [MemberData(nameof(GetAllTokensData))]
        public static void EachTokenLexes(TestToken token)
        {
            AssertLexesCorrectly(token);
        }

        [Fact]
        [Category("Lexer")]
        public void CanGetAllTokensData()
        {
            GetAllTokensData();
        }

        public static TheoryData<TestToken> GetAllTokensData()
        {
            return LexingData.GetTheoryData(LexingData.Instance.AllTokens);
        }

        [Theory]
        [Category("Lexer")]
        [MemberData(nameof(GetTwoTokenSequenceData))]
        public static void SequenceOf2TokensLexes(TestTokenSequence tokens)
        {
            AssertLexesCorrectly(tokens);
        }

        [Fact]
        [Category("Lexer")]
        public void CanGetTwoTokenSequenceData()
        {
            GetTwoTokenSequenceData();
        }

        public static TheoryData<TestTokenSequence> GetTwoTokenSequenceData()
        {
            return LexingData.GetTheoryData(LexingData.Instance.TwoTokenSequences);
        }

        [Fact]
        [Category("Lexer")]
        [Category("Slow")]
        public void SequencesOf3TokensLexes()
        {
            var sequences = LexingData.Instance.ThreeTokenSequences;
            output.WriteLine($"Sequence Count={sequences.Count:n0}");
            foreach (var sequence in sequences)
                AssertLexesCorrectly(sequence);
        }

        [Fact]
        [Category("Lexer")]
        public void CanGetThreeTokenSequences()
        {
            var _ = LexingData.Instance.ThreeTokenSequences;
        }


        [Fact]
        [Category("Lexer")]
        [Category("Slow")]
        public void SequencesOf4TokensLexes()
        {
            var sequences = LexingData.Instance.FourTokenSequences;
            output.WriteLine($"Sequence Count={sequences.Count:n0}");
            foreach (var sequence in sequences)
                AssertLexesCorrectly(sequence);
        }

        [Fact]
        [Category("Lexer")]
        public void CanGetFourTokenSequences()
        {
            var _ = LexingData.Instance.FourTokenSequences;
        }

        internal static void AssertLexesCorrectly(TestToken token)
        {
            AssertLexesCorrectly(TestTokenSequence.Single(token));
        }

        internal static void AssertLexesCorrectly(TestTokenSequence sequence)
        {
            var code = new CodeText(sequence.ToString());
            var lexer = new Lexer();
            var tokens = lexer.Lex(code);
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
                    var stringLiteralToken = Assert.IsType<StringLiteralToken>(token);
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
