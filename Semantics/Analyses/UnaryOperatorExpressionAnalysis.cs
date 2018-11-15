using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class UnaryOperatorExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new UnaryExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis Operand { get; }

        public UnaryOperatorExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] UnaryExpressionSyntax syntax,
            [NotNull] ExpressionAnalysis operand)
            : base(context, syntax.Span)
        {
            Requires.NotNull(nameof(operand), operand);
            Syntax = syntax;
            Operand = operand;
        }
    }
}
