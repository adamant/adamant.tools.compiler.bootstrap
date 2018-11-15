using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class UnaryExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Operand { get; }
        [NotNull] public IOperatorToken Operator { get; }

        public UnaryExpressionSyntax(
            [NotNull] IOperatorToken @operator,
            [NotNull] ExpressionSyntax operand)
            : base(TextSpan.Covering(@operator.Span, operand.Span))
        {
            Operator = @operator;
            Operand = operand;
        }

        public override string ToString()
        {
            return $"{Operator}{Operand}";
        }
    }
}
