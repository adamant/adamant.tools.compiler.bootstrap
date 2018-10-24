using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations
{
    public class TypeDeclarationAnalysis : MemberDeclarationAnalysis
    {
        [NotNull] public new Metatype Type { get; }
        [NotNull] public new MemberDeclarationSyntax Syntax { get; }

        public TypeDeclarationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] MemberDeclarationSyntax syntax,
            [NotNull] QualifiedName name)
            : base(context, syntax, name)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Syntax = syntax;
            Type = new Metatype(name);
        }

        [CanBeNull]
        protected override DataType GetDataType()
        {
            return Type;
        }

        [NotNull]
        public override Declaration Complete([NotNull] DiagnosticsBuilder diagnostics)
        {
            CompleteDiagnostics(diagnostics);
            return new TypeDeclaration(Context.File, Name);
        }
    }
}
