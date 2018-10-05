using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators
{
    public class BinaryOperatorExpressionSyntax : OperatorExpressionSyntax
    {
        public ExpressionSyntax LeftOperand { get; }
        public ExpressionSyntax RightOperand { get; }

        public BinaryOperatorExpressionSyntax(ExpressionSyntax leftOperand, SimpleToken @operator, ExpressionSyntax rightOperand)
            : base(@operator)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }
    }
}
