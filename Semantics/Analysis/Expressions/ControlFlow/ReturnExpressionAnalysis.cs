using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.ControlFlow;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.ControlFlow
{
    public class ReturnExpressionAnalysis : ExpressionAnalysis
    {
        [CanBeNull] public ExpressionAnalysis ReturnExpression { get; }

        public ReturnExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ReturnExpressionSyntax syntax,
            [CanBeNull] ExpressionAnalysis returnExpression)
            : base(context, syntax)
        {
            ReturnExpression = returnExpression;
        }
    }
}
