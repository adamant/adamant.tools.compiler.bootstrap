using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class AssignmentExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax LeftOperand { get; }
        public AssignmentOperation Operation { get; }
        [NotNull] public ExpressionSyntax RightOperand { get; set; }

        public AssignmentExpressionSyntax(
            [NotNull] ExpressionSyntax leftOperand,
            AssignmentOperation operation,
            [NotNull] ExpressionSyntax rightOperand)
            : base(TextSpan.Covering(leftOperand.Span, rightOperand.Span))
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
            Operation = operation;
        }

        public override string ToString()
        {
            return $"{LeftOperand} {Operation} {RightOperand}";
        }
    }
}
