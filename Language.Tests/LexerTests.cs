using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing;
using Xunit;
using Xunit.Abstractions;

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
        [MemberData(nameof(GetAllTokensData))]
        public static void EachTokenLexes(TestToken token)
        {
            LexAssert.LexesCorrectly(token);
        }

        public static TheoryData<TestToken> GetAllTokensData()
        {
            return LexingData.GetTheoryData(LexingData.Instance.AllTokens);
        }

        [Theory]
        [MemberData(nameof(GetTwoTokenSequenceData))]
        public static void SequenceOf2TokensLexes(TestTokenSequence tokens)
        {
            LexAssert.LexesCorrectly(tokens);
        }

        public static TheoryData<TestTokenSequence> GetTwoTokenSequenceData()
        {
            return LexingData.GetTheoryData(LexingData.Instance.TwoTokenSequences);
        }

        [Fact]
        public void SequenceOf3TokensLexes()
        {
            var sequences = LexingData.Instance.ThreeTokenSequences;
            output.WriteLine($"Sequence Count={sequences.Count:n0}");
            foreach (var sequence in sequences)
                LexAssert.LexesCorrectly(sequence);
        }

        [Fact]
        public void SequenceOf4TokensLexes()
        {
            var sequences = LexingData.Instance.FourTokenSequences;
            output.WriteLine($"Sequence Count={sequences.Count:n0}");
            foreach (var sequence in sequences)
                LexAssert.LexesCorrectly(sequence);
        }
    }
}
