using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.Operators
{
    public class AddExpression : BinaryOperatorExpression
    {
        public AddExpression(BinaryOperatorExpressionSyntax syntax, Expression leftOperand, Expression rightOperand, DataType type)
            : base(syntax, leftOperand, rightOperand, type)
        {
        }
    }
}
