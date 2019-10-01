using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IBinaryOperatorExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax LeftOperand { get; }
        BinaryOperator Operator { get; }
        ref IExpressionSyntax RightOperand { get; }
    }
}
