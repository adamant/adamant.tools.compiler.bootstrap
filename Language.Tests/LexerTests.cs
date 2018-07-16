using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Data;
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
            Assert.LexesCorrectly(token);
        }

        public static TheoryData<TestToken> GetAllTokensData()
        {
            return LexingData.GetTheoryData(LexingData.Instance.AllTokens);
        }

        [Theory]
        [MemberData(nameof(GetTwoTokenSequenceData))]
        public static void SequenceOf2TokensLexes(TestTokenSequence tokens)
        {
            Assert.LexesCorrectly(tokens);
        }

        public static TheoryData<TestTokenSequence> GetTwoTokenSequenceData()
        {
            return LexingData.GetTheoryData(LexingData.Instance.TwoTokenSequences);
        }

        [Fact]
        public void SequencesOf3TokensLexes()
        {
            var sequences = LexingData.Instance.ThreeTokenSequences;
            output.WriteLine($"Sequence Count={sequences.Count:n0}");
            foreach (var sequence in sequences)
                Assert.LexesCorrectly(sequence);
        }

        [Fact]
        public void SequencesOf4TokensLexes()
        {
            var sequences = LexingData.Instance.FourTokenSequences;
            output.WriteLine($"Sequence Count={sequences.Count:n0}");
            foreach (var sequence in sequences)
                Assert.LexesCorrectly(sequence);
        }
    }
}
