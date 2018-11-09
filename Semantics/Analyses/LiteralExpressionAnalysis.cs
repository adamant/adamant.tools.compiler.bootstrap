using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public abstract class LiteralExpressionAnalysis : ExpressionAnalysis
    {
        protected LiteralExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] LiteralExpressionSyntax syntax)
            : base(context, syntax)
        {
        }
    }
}
