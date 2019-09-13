using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class AssignmentExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax LeftOperand;
        public AssignmentOperator Operator { get; }
        public ExpressionSyntax RightOperand;

        public AssignmentExpressionSyntax(
            ExpressionSyntax leftOperand,
            AssignmentOperator @operator,
            ExpressionSyntax rightOperand)
            : base(TextSpan.Covering(leftOperand.Span, rightOperand.Span))
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
            Operator = @operator;
        }

        public override string ToString()
        {
            return $"{LeftOperand} {Operator.ToSymbolString()} {RightOperand}";
        }
    }
}
