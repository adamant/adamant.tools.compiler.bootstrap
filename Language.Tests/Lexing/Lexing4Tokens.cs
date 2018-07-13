using Xunit;
using Xunit.Abstractions;

namespace Adamant.Tools.Compiler.Bootstrap.Language.Tests.Lexing
{
    public class Lexing4Tokens
    {
        private readonly ITestOutputHelper output;

        public Lexing4Tokens(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void LexesAllValidTokenCombinations()
        {
            var sequences = LexingData.Instance.FourTokenSequences;
            output.WriteLine($"Sequence Count={sequences.Count:n0}");
            foreach (var sequence in sequences)
                LexAssert.LexesCorrectly(sequence);
        }
    }
}
