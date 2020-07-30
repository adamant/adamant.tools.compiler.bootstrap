using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IBinaryOperatorExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax LeftOperand { get; }
        BinaryOperator Operator { get; }
        ref IExpressionSyntax RightOperand { get; }
    }
}
