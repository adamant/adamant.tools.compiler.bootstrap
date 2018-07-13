using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public class Lexing2Tokens
    {
        [Theory]
        [MemberData(nameof(GetTwoTokenSequenceData))]
        public void LexesAllValidTokenCombinations(TestTokenSequence tokens)
        {
            LexAssert.LexesCorrectly(tokens);
        }

        public static TheoryData<TestTokenSequence> GetTwoTokenSequenceData()
        {
            return LexingData.GetTheoryData(LexingData.Instance.TwoTokenSequences);
        }
    }
}
