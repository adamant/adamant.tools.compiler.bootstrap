using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class BinaryExpressionSyntax : ExpressionSyntax, IBinaryExpressionSyntax
    {
        private IExpressionSyntax leftOperand;
        public IExpressionSyntax LeftOperand => leftOperand;
        public ref IExpressionSyntax LeftOperandRef => ref leftOperand;

        public BinaryOperator Operator { get; }

        private IExpressionSyntax rightOperand;
        public IExpressionSyntax RightOperand => rightOperand;
        public ref IExpressionSyntax RightOperandRef => ref rightOperand;

        public BinaryExpressionSyntax(
            IExpressionSyntax leftOperand,
            BinaryOperator @operator,
            IExpressionSyntax rightOperand)
            : base(TextSpan.Covering(leftOperand.Span, rightOperand.Span))
        {
            this.leftOperand = leftOperand;
            Operator = @operator;
            this.rightOperand = rightOperand;
        }

        public override string ToString()
        {
            return $"{LeftOperand} {Operator} {RightOperand}";
        }
    }
}
