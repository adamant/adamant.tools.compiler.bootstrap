using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators
{
    public class OperatorExpressionSyntax : ExpressionSyntax
    {
        public Token Operator { get; }

        public OperatorExpressionSyntax(ExpressionSyntax leftHand, Token @operator, ExpressionSyntax rightHand)
            : base(leftHand, @operator, rightHand)
        {
            Requires.That(nameof(@operator), @operator.Kind.IsOperator());
            Operator = @operator;
        }
    }
}
