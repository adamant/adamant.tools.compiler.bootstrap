using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IBinaryOperatorExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax LeftOperand { get; }
        BinaryOperator Operator { get; }
        ref IExpressionSyntax RightOperand { get; }
    }
}
