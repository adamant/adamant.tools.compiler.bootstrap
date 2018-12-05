using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class AssignmentExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax LeftOperand;
        public AssignmentOperation Operation { get; }
        public ExpressionSyntax RightOperand;

        public AssignmentExpressionSyntax(
            ExpressionSyntax leftOperand,
            AssignmentOperation operation,
            ExpressionSyntax rightOperand)
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
