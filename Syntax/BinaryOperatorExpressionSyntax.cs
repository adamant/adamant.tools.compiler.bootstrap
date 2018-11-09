using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class BinaryOperatorExpressionSyntax : OperatorExpressionSyntax
    {
        [NotNull] public ExpressionSyntax LeftOperand { get; }
        [NotNull] public ExpressionSyntax RightOperand { get; }

        public BinaryOperatorExpressionSyntax(
            [NotNull] ExpressionSyntax leftOperand,
            [NotNull] OperatorToken @operator,
            [NotNull] ExpressionSyntax rightOperand)
            : base(@operator, TextSpan.Covering(leftOperand.Span, rightOperand.Span))
        {
            Requires.NotNull(nameof(leftOperand), leftOperand);
            Requires.NotNull(nameof(rightOperand), rightOperand);
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }
    }
}
