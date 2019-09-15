using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class UnaryExpressionSyntax : ExpressionSyntax
    {
        public UnaryOperatorFixity Fixity { get; }
        public UnaryOperator Operator { get; }
        public ExpressionSyntax Operand { get; }

        public UnaryExpressionSyntax(
            TextSpan span,
            UnaryOperatorFixity fixity,
            UnaryOperator @operator,
            ExpressionSyntax operand)
            : base(span)
        {
            Operator = @operator;
            Operand = operand;
            Fixity = fixity;
        }

        public override string ToString()
        {
            switch (Fixity)
            {
                case UnaryOperatorFixity.Prefix:
                    return $"{Operator.ToSymbolString()}{Operand}";
                case UnaryOperatorFixity.Postfix:
                    return $"{Operand}{Operator.ToSymbolString()}";
                default:
                    throw ExhaustiveMatch.Failed(Fixity);
            }
        }
    }
}
