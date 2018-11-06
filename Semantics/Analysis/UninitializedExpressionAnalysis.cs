using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
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
