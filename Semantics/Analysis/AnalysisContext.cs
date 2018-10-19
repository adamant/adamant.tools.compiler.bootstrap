using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class AnalysisContext
    {
        [NotNull] public CodeFile File { get; }
        [NotNull] public LexicalScope Scope { get; }

        public AnalysisContext([NotNull] CodeFile file, [NotNull] LexicalScope scope)
        {
            Requires.NotNull(nameof(file), file);
            Requires.NotNull(nameof(scope), scope);
            File = file;
            Scope = scope;
        }
    }
}
