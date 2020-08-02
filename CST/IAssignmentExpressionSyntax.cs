namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IAssignmentExpressionSyntax : IExpressionSyntax
    {
        ref IAssignableExpressionSyntax LeftOperand { get; }
        AssignmentOperator Operator { get; }
        ref IExpressionSyntax RightOperand { get; }
    }
}
