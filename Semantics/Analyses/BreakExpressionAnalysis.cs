using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class BreakExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new BreakExpressionSyntax Syntax { get; }
        [CanBeNull] public ExpressionAnalysis Expression { get; }

        public BreakExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] BreakExpressionSyntax syntax,
            [CanBeNull] ExpressionAnalysis expression)
            : base(context, syntax.Span)
        {
            Syntax = syntax;
            Expression = expression;
        }
    }
}
