using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public class Lexing1Token
    {
        [Theory]
        [MemberData(nameof(GetOneTokenSequenceData))]
        internal void LexesAllTokens(string text, TestToken[] expectedTokens)
        {
            LexAssert.LexesTo(text, expectedTokens);
        }

        public static IEnumerable<object[]> GetOneTokenSequenceData()
        {
            return LexingData.Instance.OneTokenSequences.Select(TestToken.GetSequenceData);
        }
    }
}
