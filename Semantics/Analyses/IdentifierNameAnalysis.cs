using Adamant.Tools.Compiler.Bootstrap.AST;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class IdentifierNameAnalysis : SimpleNameAnalysis
    {
        [NotNull] public new IdentifierNameSyntax Syntax { get; }

        public IdentifierNameAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] IdentifierNameSyntax syntax)
            : base(context, syntax)
        {
            Syntax = syntax;
        }
    }
}
