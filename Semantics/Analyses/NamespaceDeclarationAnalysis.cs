using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class NamespaceDeclarationAnalysis : DeclarationAnalysis
    {
        [NotNull] public new NamespaceDeclarationSyntax Syntax { get; }
        [NotNull, ItemNotNull] public FixedList<DeclarationAnalysis> Declarations { get; }

        public NamespaceDeclarationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] NamespaceDeclarationSyntax syntax,
            [NotNull, ItemNotNull] IEnumerable<DeclarationAnalysis> declarations)
            : base(context, syntax)
        {
            Syntax = syntax;
            Declarations = declarations.ToFixedList();
        }
    }
}
