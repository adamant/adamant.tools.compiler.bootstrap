using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements
{
    public abstract class StatementAnalysis : AnalysisNode
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