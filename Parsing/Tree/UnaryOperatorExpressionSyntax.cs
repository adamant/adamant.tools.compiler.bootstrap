using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class UnaryOperatorExpressionSyntax : ExpressionSyntax, IUnaryOperatorExpressionSyntax
    {
        public UnaryOperatorFixity Fixity { get; }
        public UnaryOperator Operator { get; }
        private IExpressionSyntax operand;
        public ref IExpressionSyntax Operand => ref operand;

        public UnaryOperatorExpressionSyntax(
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
                default:
                    throw ExhaustiveMatch.Failed(Fixity);
                case UnaryOperatorFixity.Prefix:
                    return $"{Operator.ToSymbolString()}{Operand}";
                case UnaryOperatorFixity.Postfix:
                    return $"{Operand}{Operator.ToSymbolString()}";

            }
        }
    }
}
