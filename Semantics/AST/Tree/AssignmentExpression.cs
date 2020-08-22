using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Operators;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class AssignmentExpression : Expression, IAssignmentExpression
    {
        public IAssignableExpression LeftOperand { [DebuggerStepThrough] get; }
        public AssignmentOperator Operator { [DebuggerStepThrough] get; }
        public IExpression RightOperand { [DebuggerStepThrough] get; }

        public AssignmentExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            IAssignableExpression leftOperand,
            AssignmentOperator @operator,
            IExpression rightOperand)
            : base(span, dataType, semantics)
        {
            LeftOperand = leftOperand;
            Operator = @operator;
            RightOperand = rightOperand;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Assignment;

        public override string ToString()
        {
            return $"{LeftOperand.ToGroupedString(ExpressionPrecedence)} {Operator.ToSymbolString()} {RightOperand.ToGroupedString(ExpressionPrecedence)}";
        }
    }
}
