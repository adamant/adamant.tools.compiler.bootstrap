using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations
{
    public abstract class MemberDeclarationAnalysis : IDeclarationAnalysis
    {
        [NotNull] public AnalysisContext Context { get; }
        [NotNull] public MemberDeclarationSyntax Syntax { get; }
        [NotNull] public DiagnosticsBuilder Diagnostics { get; }
        [NotNull] public QualifiedName Name { get; }
        // This is the type of the value provided by using the name. So for
        // classes, it is the metatype, for functions, the function type
        [CanBeNull] public DataType Type => GetDataType();

        protected MemberDeclarationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] MemberDeclarationSyntax syntax,
            [NotNull] QualifiedName name)
        {
            Requires.NotNull(nameof(context), context);
            Requires.NotNull(nameof(syntax), syntax);
            Requires.NotNull(nameof(name), name);
            Context = context;
            Syntax = syntax;
            Diagnostics = new DiagnosticsBuilder();
            Name = name;
        }

        [CanBeNull]
        protected abstract DataType GetDataType();

        [NotNull]
        public abstract Declaration Complete([NotNull] DiagnosticsBuilder diagnostics);

        protected void CompleteDiagnostics([NotNull] DiagnosticsBuilder diagnostics)
        {
            diagnostics.Publish(Diagnostics.Build());
        }
    }
}
