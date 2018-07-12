using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public class Lexing3Tokens
    {
        [Theory]
        [MemberData(nameof(GetThreeTokenSequenceData))]
        internal void LexesAllValidTokenCombinations(string text, TestToken[] expectedTokens)
        {
            LexAssert.LexesTo(text, expectedTokens);
        }

        public static IEnumerable<object[]> GetThreeTokenSequenceData()
        {
            return LexingData.Instance.ThreeTokenSequences.Select(TestToken.GetSequenceData);
        }
    }
}
