using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators
{
    public class UnaryOperatorExpressionSyntax : OperatorExpressionSyntax
    {
        public ExpressionSyntax Operand { get; }

        public UnaryOperatorExpressionSyntax(SimpleToken @operator, ExpressionSyntax operand)
            : base(@operator)
        {
            Operand = operand;
        }
    }
}
