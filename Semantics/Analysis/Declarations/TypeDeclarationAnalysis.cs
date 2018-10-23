using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations
{
    public class TypeDeclarationAnalysis : DeclarationAnalysis
    {
        [NotNull] public DeclarationSyntax Syntax { get; }

        public TypeDeclarationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] DeclarationSyntax syntax,
            [NotNull] QualifiedName qualifiedName)
            : base(context, qualifiedName)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Syntax = syntax;
        }

        [NotNull]
        public override Declaration Complete([NotNull] DiagnosticsBuilder diagnostics)
        {
            CompleteDiagnostics(diagnostics);
            return new TypeDeclaration(Context.File, QualifiedName);
        }
    }
}
