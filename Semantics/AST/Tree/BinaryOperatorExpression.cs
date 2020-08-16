using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Operators;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class BinaryOperatorExpression : Expression, IBinaryOperatorExpression
    {
        public IExpression LeftOperand { get; }
        public BinaryOperator Operator { get; }
        public IExpression RightOperand { get; }

        public BinaryOperatorExpression(
            TextSpan span,
            DataType dataType,
            IExpression leftOperand,
            BinaryOperator @operator,
            IExpression rightOperand)
            : base(span, dataType)
        {
            LeftOperand = leftOperand;
            Operator = @operator;
            RightOperand = rightOperand;
        }
    }
}
