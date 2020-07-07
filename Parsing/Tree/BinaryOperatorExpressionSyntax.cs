using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using ExhaustiveMatching;

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

        protected override OperatorPrecedence ExpressionPrecedence => Operator switch
        {
            BinaryOperator.And => OperatorPrecedence.LogicalAnd,
            BinaryOperator.Or => OperatorPrecedence.LogicalOr,

            BinaryOperator.Plus => OperatorPrecedence.Additive,
            BinaryOperator.Minus => OperatorPrecedence.Additive,

            BinaryOperator.Asterisk => OperatorPrecedence.Multiplicative,
            BinaryOperator.Slash => OperatorPrecedence.Multiplicative,

            BinaryOperator.DotDot => OperatorPrecedence.Range,
            BinaryOperator.DotDotLessThan => OperatorPrecedence.Range,
            BinaryOperator.LessThanDotDot => OperatorPrecedence.Range,
            BinaryOperator.LessThanDotDotLessThan => OperatorPrecedence.Range,

            BinaryOperator.EqualsEquals => OperatorPrecedence.Relational,
            BinaryOperator.NotEqual => OperatorPrecedence.Relational,
            BinaryOperator.LessThan => OperatorPrecedence.Relational,
            BinaryOperator.LessThanOrEqual => OperatorPrecedence.Relational,
            BinaryOperator.GreaterThan => OperatorPrecedence.Relational,
            BinaryOperator.GreaterThanOrEqual => OperatorPrecedence.Relational,

            _ => throw ExhaustiveMatch.Failed(Operator),
        };

        public override string ToString()
        {
            return $"{LeftOperand.ToGroupedString(ExpressionPrecedence)} {Operator.ToSymbolString()} {RightOperand.ToGroupedString(ExpressionPrecedence)}";
        }
    }
}
