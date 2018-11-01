using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Namespaces;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations
{
    public class NamespaceDeclarationAnalysis : DeclarationAnalysis
    {
        [NotNull] public new NamespaceDeclarationSyntax Syntax { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<DeclarationAnalysis> Declarations { get; }

        public NamespaceDeclarationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] NamespaceDeclarationSyntax syntax,
            [NotNull] [ItemNotNull] IEnumerable<DeclarationAnalysis> declarations)
            : base(context, syntax)
        {
            Syntax = syntax;
            Declarations = declarations.ToReadOnlyList();
        }
    }
}
