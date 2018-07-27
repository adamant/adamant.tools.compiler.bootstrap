using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.Operators
{
    public abstract class BinaryOperatorExpression : OperatorExpression
    {
        public new BinaryOperatorExpressionSyntax Syntax { get; }
        public Expression LeftOperand { get; }
        public Expression RightOperand { get; }

        protected BinaryOperatorExpression(BinaryOperatorExpressionSyntax syntax, Expression leftOperand, Expression rightOperand)
        {
            Syntax = syntax;
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
