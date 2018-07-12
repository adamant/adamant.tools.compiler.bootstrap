using System.Linq;
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
        internal void LexesAllValidTokenCombinations()
        {
            var sequences = LexingData.Instance.ThreeTokenSequences;
            output.WriteLine($"Sequence Count={sequences.Count}");
            foreach (var sequence in sequences)
                LexAssert.LexesTo(string.Concat(sequence.Select(s => s.Text)), sequence.Where(s => s.Kind.Category == TestTokenCategory.Token));
        }
    }
}
