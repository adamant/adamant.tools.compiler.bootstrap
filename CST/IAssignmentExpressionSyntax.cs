namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IAssignmentExpressionSyntax
    {
        ref IAssignableExpressionSyntax LeftOperand { get; }
        ref IExpressionSyntax RightOperand { get; }
    }
}
