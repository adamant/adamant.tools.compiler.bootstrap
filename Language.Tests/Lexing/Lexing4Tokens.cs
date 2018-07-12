using System.Linq;
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
        internal void LexesAllValidTokenCombinations()
        {
            var sequences = LexingData.Instance.FourTokenSequences;
            output.WriteLine($"Sequence Count={sequences.Count}");
            foreach (var sequence in sequences)
                LexAssert.LexesTo(string.Concat(sequence.Select(s => s.Text)), sequence.Where(s => s.Kind.Category == TestTokenCategory.Token));
        }
    }
}
