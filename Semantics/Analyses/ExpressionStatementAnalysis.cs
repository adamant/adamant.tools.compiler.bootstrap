using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class ExpressionStatementAnalysis : StatementAnalysis
    {
        [NotNull] public new ExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis Expression { get; }

        public ExpressionStatementAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ExpressionSyntax syntax,
            [NotNull] ExpressionAnalysis expression)
            : base(context, syntax)
        {
            Syntax = syntax;
            Expression = expression;
        }
    }
}
