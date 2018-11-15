using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class BinaryOperatorExpressionSyntax : OperatorExpressionSyntax
    {
        [NotNull] public ExpressionSyntax LeftOperand { get; }
        [NotNull] public ExpressionSyntax RightOperand { get; }

        public BinaryOperatorExpressionSyntax(
            [NotNull] ExpressionSyntax leftOperand,
            [NotNull] IOperatorToken @operator,
            [NotNull] ExpressionSyntax rightOperand)
            : base(@operator, TextSpan.Covering(leftOperand.Span, rightOperand.Span))
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        public override string ToString()
        {
            return $"{LeftOperand} {Operator} {RightOperand}";
        }
    }
}
