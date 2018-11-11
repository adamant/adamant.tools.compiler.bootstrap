using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class UninitializedExpressionAnalysis : LiteralExpressionAnalysis
    {
        public UninitializedExpressionAnalysis(
            [NotNull] AnalysisContext context,
            TextSpan span)
            : base(context, span)
        {
        }
    }
}
