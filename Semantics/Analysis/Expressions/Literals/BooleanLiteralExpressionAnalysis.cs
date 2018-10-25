using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Literals;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions.Literals
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
