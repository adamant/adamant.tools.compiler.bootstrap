using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class UnaryExpressionSyntax : ExpressionSyntax, IUnaryExpressionSyntax
    {
        public UnaryOperatorFixity Fixity { get; }
        public UnaryOperator Operator { get; }
        private IExpressionSyntax operand;
        public IExpressionSyntax Operand => operand;
        public ref IExpressionSyntax OperandRef => ref operand;

        public UnaryExpressionSyntax(
            TextSpan span,
            UnaryOperatorFixity fixity,
            UnaryOperator @operator,
            IExpressionSyntax operand)
            : base(span)
        {
            Operator = @operator;
            this.operand = operand;
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
