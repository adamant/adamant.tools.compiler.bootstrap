using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class ReturnExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new ReturnExpressionSyntax Syntax { get; }
        [CanBeNull] public ExpressionAnalysis ReturnValue { get; }

        public ReturnExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ReturnExpressionSyntax syntax,
            [CanBeNull] ExpressionAnalysis returnValue)
            : base(context, syntax.Span)
        {
            Syntax = syntax;
            ReturnValue = returnValue;
        }
    }
}
