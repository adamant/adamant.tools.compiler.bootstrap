using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class ResultExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new ResultExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis Expression { get; }

        public ResultExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ResultExpressionSyntax syntax,
            [NotNull] ExpressionAnalysis expression)
           : base(context, syntax)
        {
            Requires.NotNull(nameof(expression), expression);
            Syntax = syntax;
            Expression = expression;
        }
    }
}
