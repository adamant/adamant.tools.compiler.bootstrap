using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IBinaryExpressionSyntax : IExpressionSyntax
    {
        IExpressionSyntax LeftOperand { get; }
        ref IExpressionSyntax LeftOperandRef { get; }
        BinaryOperator Operator { get; }
        IExpressionSyntax RightOperand { get; }
        ref IExpressionSyntax RightOperandRef { get; }
    }
}
