using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations
{
    public abstract class MemberDeclarationAnalysis : AnalysisNode, IDeclarationAnalysis
    {
        [NotNull] public new MemberDeclarationSyntax Syntax { get; }
        [NotNull] public DiagnosticsBuilder Diagnostics { get; }
        [NotNull] public QualifiedName Name { get; }
        // This is the type of the value provided by using the name. So for
        // classes, it is the metatype, for functions, the function type
        [CanBeNull] public DataType Type => GetDataType();

        protected MemberDeclarationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] MemberDeclarationSyntax syntax,
            [NotNull] QualifiedName name)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(name), name);
            Syntax = syntax;
            Diagnostics = new DiagnosticsBuilder();
            Name = name;
        }

        [CanBeNull]
        protected abstract DataType GetDataType();

        [CanBeNull]
        public abstract Declaration Complete([NotNull] DiagnosticsBuilder diagnostics);

        protected bool CompleteDiagnostics([NotNull] DiagnosticsBuilder diagnostics)
        {
            var errors = Diagnostics.Any(d => d.Level >= DiagnosticLevel.CompilationError);
            diagnostics.Publish(Diagnostics.Build());
            return errors;
        }
    }
}
