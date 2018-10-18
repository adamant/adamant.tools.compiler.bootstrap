using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations
{
    public abstract class DeclarationAnalysis
    {
        [NotNull] public CodeFile File { get; }
        [NotNull] public LexicalScope Scope { get; }
        [NotNull] public QualifiedName QualifiedName { get; }

        protected DeclarationAnalysis(
            [NotNull] CodeFile file,
            [NotNull] LexicalScope scope,
            [NotNull] QualifiedName qualifiedName)
        {
            Requires.NotNull(nameof(file), file);
            Requires.NotNull(nameof(scope), scope);
            Requires.NotNull(nameof(qualifiedName), qualifiedName);
            File = file;
            Scope = scope;
            QualifiedName = qualifiedName;
        }

        [NotNull]
        public abstract Declaration Complete();
    }
}
