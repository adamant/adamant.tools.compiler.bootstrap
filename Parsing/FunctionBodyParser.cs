using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public partial class FunctionBodyParser
    {
        [NotNull] private readonly IListParser listParser;

        public FunctionBodyParser([NotNull] IListParser listParser)
        {
            this.listParser = listParser;
        }
    }
}
