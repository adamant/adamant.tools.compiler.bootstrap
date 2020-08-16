using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Operators;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class UnaryOperatorExpression : Expression, IUnaryOperatorExpression
    {
        public UnaryOperatorFixity Fixity { get; }
        public UnaryOperator Operator { get; }
        public IExpression Operand { get; }

        public UnaryOperatorExpression(
            TextSpan span,
            DataType dataType,
            UnaryOperatorFixity fixity,
            UnaryOperator @operator,
            IExpression operand)
            : base(span, dataType)
        {
            Fixity = fixity;
            Operator = @operator;
            Operand = operand;
        }
    }
}
