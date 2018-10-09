using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators
{
    public class OperatorExpressionSyntax : ExpressionSyntax
    {
        [NotNull]
        public OperatorToken Operator { get; }

        public OperatorExpressionSyntax([NotNull] OperatorToken @operator)
        {
            Requires.NotNull(nameof(@operator), @operator);
            Operator = @operator;
        }
    }
}
