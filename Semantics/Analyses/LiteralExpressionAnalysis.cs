using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public abstract class LiteralExpressionAnalysis : ExpressionAnalysis
    {
        protected LiteralExpressionAnalysis(
            [NotNull] AnalysisContext context,
            TextSpan span)
            : base(context, span)
        {
        }
    }
}
