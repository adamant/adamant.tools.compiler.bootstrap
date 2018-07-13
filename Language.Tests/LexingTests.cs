using Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing;
using Xunit;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests
{
    public class LexingTests
    {
        private readonly ITestOutputHelper output;

        public LexingTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [MemberData(nameof(GetTestSequenceData))]
        public static void SequenceLexes(TestTokenSequence tokens)
        {
            LexAssert.LexesCorrectly(tokens);
        }

        public static TheoryData<TestTokenSequence> GetTestSequenceData()
        {
            return LexingData.GetTheoryData(LexingData.Instance.Tests);
        }

        [Theory]
        [MemberData(nameof(GetOneTokenSequenceData))]
        public static void EachTokenLexes(TestTokenSequence tokens)
        {
            LexAssert.LexesCorrectly(tokens);
        }

        public static TheoryData<TestTokenSequence> GetOneTokenSequenceData()
        {
            return LexingData.GetTheoryData(LexingData.Instance.OneTokenSequences);
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
