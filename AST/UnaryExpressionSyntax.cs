using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class UnaryExpressionSyntax : ExpressionSyntax
    {
        public UnaryOperatorFixity Fixty { get; }
        public UnaryOperator Operator { get; }
        public ExpressionSyntax Operand { get; }

        public UnaryExpressionSyntax(
            TextSpan span,
            UnaryOperatorFixity fixty,
            UnaryOperator @operator,
            ExpressionSyntax operand)
            : base(span)
        {
            Operator = @operator;
            Operand = operand;
            Fixty = fixty;
        }

        public override string ToString()
        {
            switch (Fixty)
            {
                case UnaryOperatorFixity.Prefix:
                    return $"{Operator.ToSymbolString()}{Operand}";
                case UnaryOperatorFixity.Postfix:
                    return $"{Operand}{Operator.ToSymbolString()}";
                default:
                    throw NonExhaustiveMatchException.ForEnum(Fixty);
            }
        }
    }
}
