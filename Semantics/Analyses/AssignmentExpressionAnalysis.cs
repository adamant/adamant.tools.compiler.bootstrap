using Adamant.Tools.Compiler.Bootstrap.AST;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class AssignmentExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public AssignmentExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis LeftOperand { get; }
        [NotNull] public AssignmentOperation Operation => Syntax.Operation;
        [NotNull] public ExpressionAnalysis RightOperand { get; }

        public AssignmentExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] AssignmentExpressionSyntax syntax,
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
