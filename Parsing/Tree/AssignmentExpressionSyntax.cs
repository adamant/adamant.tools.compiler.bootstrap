using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class AssignmentExpressionSyntax : ExpressionSyntax, IAssignmentExpressionSyntax
    {
        private IAssignableExpressionSyntax leftOperand;
        public ref IAssignableExpressionSyntax LeftOperand => ref leftOperand;

        public AssignmentOperator Operator { get; }
        private IExpressionSyntax rightOperand;
        public ref IExpressionSyntax RightOperand => ref rightOperand;

        public AssignmentExpressionSyntax(
            IAssignableExpressionSyntax leftOperand,
            AssignmentOperator @operator,
            IExpressionSyntax rightOperand)
            : base(TextSpan.Covering(leftOperand.Span, rightOperand.Span))
        {
            this.leftOperand = leftOperand;
            this.rightOperand = rightOperand;
            Operator = @operator;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Assignment;

        public override string ToString()
        {
            return $"{LeftOperand.ToGroupedString(ExpressionPrecedence)} {Operator.ToSymbolString()} {RightOperand.ToGroupedString(ExpressionPrecedence)}";
        }
    }
}
