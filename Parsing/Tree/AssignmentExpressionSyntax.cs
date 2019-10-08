using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class AssignmentExpressionSyntax : ExpressionSyntax, IAssignmentExpressionSyntax
    {
        private IExpressionSyntax leftOperand;
        public ref IExpressionSyntax LeftOperand => ref leftOperand;

        public AssignmentOperator Operator { get; }
        public ITransferSyntax RightOperand { get; }

        public AssignmentExpressionSyntax(
            IExpressionSyntax leftOperand,
            AssignmentOperator @operator,
            ITransferSyntax rightOperand)
            : base(TextSpan.Covering(leftOperand.Span, rightOperand.Span))
        {
            this.leftOperand = leftOperand;
            RightOperand = rightOperand;
            Operator = @operator;
        }

        public override string ToString()
        {
            return $"{LeftOperand} {Operator.ToSymbolString()} {RightOperand}";
        }
    }
}
