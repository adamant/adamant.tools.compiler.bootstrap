using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators
{
    public class OperatorExpressionSyntax : ExpressionSyntax
    {
        public SimpleToken Operator { get; }

        public OperatorExpressionSyntax(SimpleToken @operator)
        {
            Requires.That(nameof(@operator), @operator.Kind.IsOperator());
            Operator = @operator;
        }
    }
}
