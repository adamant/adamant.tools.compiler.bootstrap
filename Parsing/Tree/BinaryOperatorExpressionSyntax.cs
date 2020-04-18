using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class BinaryOperatorExpressionSyntax : ExpressionSyntax, IBinaryOperatorExpressionSyntax
    {
        private IExpressionSyntax leftOperand;
        public ref IExpressionSyntax LeftOperand => ref leftOperand;

        public BinaryOperator Operator { get; }


        private IExpressionSyntax rightOperand;
        public ref IExpressionSyntax RightOperand => ref rightOperand;

        public BinaryOperatorExpressionSyntax(
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
            return $"{LeftOperand} {Operator.ToSymbolString()} {RightOperand}";
        }
    }
}
