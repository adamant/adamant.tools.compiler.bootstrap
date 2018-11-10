using Adamant.Tools.Compiler.Bootstrap.Lexing;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class Parser
    {
        [NotNull] private readonly ITokenIterator tokens;

        public Parser([NotNull] ITokenIterator tokens)
        {
            this.tokens = tokens;
        }
    }
}
