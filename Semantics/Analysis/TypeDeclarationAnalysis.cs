using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class TypeDeclarationAnalysis : MemberDeclarationAnalysis
    {
        [CanBeNull] public new DataType Type { get; set; }
        [NotNull] public new MemberDeclarationSyntax Syntax { get; }

        public TypeDeclarationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] MemberDeclarationSyntax syntax,
            [NotNull] Name name,
            [NotNull] [ItemNotNull] IEnumerable<GenericParameterAnalysis> genericParameters)
            : base(context, syntax, name, genericParameters)
        {
            Requires.NotNull(nameof(syntax), syntax);
            Syntax = syntax;
        }

        [CanBeNull]
        protected override DataType GetDataType()
        {
            return Type;
        }

        [CanBeNull]
        public override Declaration Complete([NotNull] DiagnosticsBuilder diagnostics)
        {
            if (CompleteDiagnostics(diagnostics)) return null;
            return new TypeDeclaration(Context.File, Name, Type.AssertKnown(), GenericParameters.Select(p => p.Complete()));
        }
    }
}
