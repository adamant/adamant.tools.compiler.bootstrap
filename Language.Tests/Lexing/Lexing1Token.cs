using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public class Lexing1Token
    {
        [Theory]
        [MemberData(nameof(GetOneTokenSequenceData))]
        public void LexesAllTokens(TestTokenSequence tokens)
        {
            LexAssert.LexesCorrectly(tokens);
        }

        public static TheoryData<TestTokenSequence> GetOneTokenSequenceData()
        {
            return LexingData.GetTheoryData(LexingData.Instance.OneTokenSequences);
        }
    }
}
