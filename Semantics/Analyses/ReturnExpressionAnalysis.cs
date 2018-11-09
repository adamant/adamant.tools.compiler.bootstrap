using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class ReturnExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new ReturnExpressionSyntax Syntax { get; }
        [CanBeNull] public ExpressionAnalysis ReturnExpression { get; }

        public ReturnExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ReturnExpressionSyntax syntax,
            [CanBeNull] ExpressionAnalysis returnExpression)
            : base(context, syntax)
        {
            Syntax = syntax;
            ReturnExpression = returnExpression;
        }
    }
}
