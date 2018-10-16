using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public abstract class DeclarationAnalysis
    {
        [NotNull] public LexicalScope Scope { get; }
        [NotNull] public CodeFile File { get; }
        [NotNull] public Declaration Semantics { get; }

        protected DeclarationAnalysis(
            [NotNull] CodeFile file,
            [NotNull] LexicalScope scope,
            [NotNull] Declaration semantics)
        {
            Requires.NotNull(nameof(file), file);
            Requires.NotNull(nameof(scope), scope);
            Requires.NotNull(nameof(semantics), semantics);
            File = file;
            Scope = scope;
            Semantics = semantics;
        }
    }
}
