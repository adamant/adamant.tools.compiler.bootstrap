using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations
{
    public abstract class DeclarationAnalysis
    {
        [NotNull] public AnalysisContext Context { get; }
        [NotNull] public DiagnosticsBuilder Diagnostics { get; }
        [NotNull] public QualifiedName QualifiedName { get; }

        protected DeclarationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] QualifiedName qualifiedName)
        {
            Requires.NotNull(nameof(context), context);
            Requires.NotNull(nameof(qualifiedName), qualifiedName);
            Diagnostics = new DiagnosticsBuilder();
            QualifiedName = qualifiedName;
            Context = context;
        }

        [NotNull]
        public abstract Declaration Complete([NotNull] DiagnosticsBuilder diagnostics);

        protected void CompleteDiagnostics([NotNull] DiagnosticsBuilder diagnostics)
        {
            diagnostics.Publish(Diagnostics.Build());
        }
    }
}
