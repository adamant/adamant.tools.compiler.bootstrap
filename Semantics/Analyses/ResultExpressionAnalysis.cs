using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class ResultExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new ResultExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis Expression { get; }

        public ResultExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ResultExpressionSyntax syntax,
            [NotNull] ExpressionAnalysis expression)
           : base(context, syntax.Span)
        {
            Requires.NotNull(nameof(expression), expression);
            Syntax = syntax;
            Expression = expression;
        }
    }
}
