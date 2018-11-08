using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public abstract class StatementAnalysis : SyntaxAnalysis
    {
        [NotNull] public new StatementSyntax Syntax { get; }

        protected StatementAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] StatementSyntax syntax)
            : base(context, syntax)
        {
            Syntax = syntax;
        }
    }
}
