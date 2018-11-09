using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class UninitializedExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new UninitializedExpressionSyntax Syntax { get; }

        public UninitializedExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] UninitializedExpressionSyntax syntax)
            : base(context, syntax)
        {
            Syntax = syntax;
        }
    }
}
