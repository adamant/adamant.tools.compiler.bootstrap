using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Core.Operators;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

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
            ExpressionSemantics semantics,
            UnaryOperatorFixity fixity,
            UnaryOperator @operator,
            IExpression operand)
            : base(span, dataType, semantics)
        {
            Fixity = fixity;
            Operator = @operator;
            Operand = operand;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Unary;

        public override string ToString()
        {
            return Fixity switch
            {
                UnaryOperatorFixity.Prefix => $"{Operator.ToSymbolString()}{Operand.ToGroupedString(ExpressionPrecedence)}",
                UnaryOperatorFixity.Postfix => $"{Operand.ToGroupedString(ExpressionPrecedence)}{Operator.ToSymbolString()}",
                _ => throw ExhaustiveMatch.Failed(Fixity)
            };
        }
    }
}
