using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class RecursiveDescentParser
    {
        protected CodeFile File { get; }
        protected ITokenIterator<IEssentialToken> Tokens { get; }

        public RecursiveDescentParser(ITokenIterator<IEssentialToken> tokens)
        {
            File = tokens.Context.File;
            Tokens = tokens;
        }

        protected void Add(Diagnostic diagnostic)
        {
            Tokens.Context.Diagnostics.Add(diagnostic);
        }
    }
}
