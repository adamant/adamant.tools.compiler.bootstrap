using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class BinaryExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax LeftOperand { get; }
        public BinaryOperator Operator { get; }
        [NotNull] public ExpressionSyntax RightOperand { get; }

        public BinaryExpressionSyntax(
            [NotNull] ExpressionSyntax leftOperand,
            BinaryOperator @operator,
            [NotNull] ExpressionSyntax rightOperand)
            : base(TextSpan.Covering(leftOperand.Span, rightOperand.Span))
        {
            LeftOperand = leftOperand;
            Operator = @operator;
            RightOperand = rightOperand;
        }

        public override string ToString()
        {
            return $"{LeftOperand} {Operator} {RightOperand}";
        }
    }
}
