using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public class Lexing2Tokens
    {
        [Theory]
        [MemberData(nameof(GetTwoTokenSequenceData))]
        internal void LexesAllValidTokenCombinations(string text, TestToken[] expectedTokens)
        {
            LexAssert.LexesTo(text, expectedTokens);
        }

        public static IEnumerable<object[]> GetTwoTokenSequenceData()
        {
            return LexingData.Instance.TwoTokenSequences.Select(TestToken.GetSequenceData);
        }
    }
}
