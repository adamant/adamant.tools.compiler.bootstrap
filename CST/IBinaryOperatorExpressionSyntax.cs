namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IBinaryOperatorExpressionSyntax
    {
        ref IExpressionSyntax LeftOperand { get; }
        ref IExpressionSyntax RightOperand { get; }
    }
}
