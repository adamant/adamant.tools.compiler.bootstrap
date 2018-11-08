using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class BooleanLiteralExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new BooleanLiteralExpressionSyntax Syntax { get; }

        public BooleanLiteralExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] BooleanLiteralExpressionSyntax syntax)
            : base(context, syntax)
        {
            Syntax = syntax;
        }
    }
}
