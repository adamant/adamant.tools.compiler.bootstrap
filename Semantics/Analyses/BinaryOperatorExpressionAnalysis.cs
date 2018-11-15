using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class BinaryOperatorExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public BinaryExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis LeftOperand { get; }
        [NotNull] public BinaryOperator Operator => Syntax.Operator;
        [NotNull] public ExpressionAnalysis RightOperand { get; }

        public BinaryOperatorExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] BinaryExpressionSyntax syntax,
            [NotNull] ExpressionAnalysis leftOperand,
            [NotNull] ExpressionAnalysis rightOperand)
            : base(context, syntax.Span)
        {
            Syntax = syntax;
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }
    }
}
