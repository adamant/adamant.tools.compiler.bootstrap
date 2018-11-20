using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    public partial class Parser
    {
        [NotNull] protected readonly CodeFile File;
        [NotNull] protected readonly ITokenIterator Tokens;
        [NotNull] private readonly RootName nameContext;

        public Parser([NotNull] ITokenIterator tokens, [NotNull] RootName nameContext)
        {
            File = tokens.Context.File;
            Tokens = tokens;
            this.nameContext = nameContext;
        }

        protected void Add([NotNull] Diagnostic diagnostic)
        {
            Tokens.Context.Diagnostics.Add(diagnostic);
        }
    }
}
