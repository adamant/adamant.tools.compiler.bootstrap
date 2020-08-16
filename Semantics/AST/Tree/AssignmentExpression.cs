using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Operators;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class AssignmentExpression : Expression, IAssignmentExpression
    {
        public IAssignableExpression LeftOperand { get; }
        public AssignmentOperator Operator { get; }
        public IExpression RightOperand { get; }

        public AssignmentExpression(TextSpan span, DataType dataType, IAssignableExpression leftOperand, AssignmentOperator @operator, IExpression rightOperand)
            : base(span, dataType)
        {
            LeftOperand = leftOperand;
            Operator = @operator;
            RightOperand = rightOperand;
        }
    }
}
