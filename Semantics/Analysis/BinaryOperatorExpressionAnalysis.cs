using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class BinaryOperatorExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new BinaryOperatorExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis LeftOperand { get; }
        [NotNull] public OperatorToken Operator => Syntax.Operator;
        [NotNull] public ExpressionAnalysis RightOperand { get; }

        public BinaryOperatorExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] BinaryOperatorExpressionSyntax syntax,
            [NotNull] ExpressionAnalysis leftOperand,
            [NotNull] ExpressionAnalysis rightOperand)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(leftOperand), leftOperand);
            Requires.NotNull(nameof(rightOperand), rightOperand);
            Syntax = syntax;
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }
    }
}
