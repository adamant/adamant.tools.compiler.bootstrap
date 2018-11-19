using Adamant.Tools.Compiler.Bootstrap.AST;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public abstract class StatementAnalysis : SyntaxAnalysis
    {
        [NotNull] public new StatementSyntax Syntax { get; }

        protected StatementAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] StatementSyntax syntax)
            : base(context)
        {
            Syntax = syntax;
        }
    }
}
