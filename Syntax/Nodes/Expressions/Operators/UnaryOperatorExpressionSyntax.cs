using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Operators
{
    public class UnaryOperatorExpressionSyntax : OperatorExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Operand { get; }

        public UnaryOperatorExpressionSyntax(
            [NotNull] OperatorToken @operator,
            [NotNull] ExpressionSyntax operand)
            : base(@operator, TextSpan.Covering(@operator.Span, operand.Span))
        {
            Requires.NotNull(nameof(operand), operand);
            Operand = operand;
        }
    }
}
