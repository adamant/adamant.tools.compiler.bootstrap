using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public abstract class ParserBase
    {
        [NotNull] protected readonly CodeFile File;
        [NotNull] protected readonly ITokenIterator Tokens;

        protected ParserBase([NotNull] ITokenIterator tokens)
        {
            File = tokens.Context.File;

            Tokens = tokens;
        }

        protected void Add([NotNull] Diagnostic diagnostic)
        {
            Tokens.Context.Diagnostics.Add(diagnostic);
        }
    }
}
