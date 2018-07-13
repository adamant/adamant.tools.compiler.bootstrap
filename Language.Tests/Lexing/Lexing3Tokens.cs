using Xunit;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public class Lexing3Tokens
    {
        private readonly ITestOutputHelper output;

        public Lexing3Tokens(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void LexesAllValidTokenCombinations()
        {
            var sequences = LexingData.Instance.ThreeTokenSequences;
            output.WriteLine($"Sequence Count={sequences.Count:n0}");
            foreach (var sequence in sequences)
                LexAssert.LexesCorrectly(sequence);
        }
    }
}
