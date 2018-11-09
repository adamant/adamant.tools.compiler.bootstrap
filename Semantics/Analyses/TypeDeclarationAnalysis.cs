using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class TypeDeclarationAnalysis : MemberDeclarationAnalysis
    {
        [NotNull] public new MemberDeclarationSyntax Syntax { get; }

        public TypeDeclarationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] MemberDeclarationSyntax syntax,
            [NotNull] Name name,
            [CanBeNull, ItemNotNull] IEnumerable<GenericParameterAnalysis> genericParameters)
            : base(context, syntax, name, genericParameters)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Syntax = syntax;
        }

        [CanBeNull]
        public override Declaration Complete([NotNull] DiagnosticsBuilder diagnostics)
        {
            if (CompleteDiagnostics(diagnostics)) return null;
            return new TypeDeclaration(Context.File, Name, Type.AssertResolved(), GenericParameters?.Select(p => p.Complete()));
        }
    }
}
