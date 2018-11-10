using Adamant.Tools.Compiler.Bootstrap.Lexing;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class FunctionBodyParser : Parser
    {
        [NotNull] private readonly IListParser listParser;

        public FunctionBodyParser(
            [NotNull] ITokenIterator tokens,
            [NotNull] IListParser listParser)
            : base(tokens)
        {
            this.listParser = listParser;
        }
    }
}
