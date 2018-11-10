using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public class Parser
    {
        [NotNull] protected readonly CodeFile File;
        [NotNull] protected readonly ITokenIterator Tokens;

        public Parser([NotNull] ITokenIterator tokens)
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