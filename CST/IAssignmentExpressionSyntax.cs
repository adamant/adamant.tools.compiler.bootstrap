namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IAssignmentExpressionSyntax : IExpressionSyntax
    {
        ref IAssignableExpressionSyntax LeftOperand { get; }
        AssignmentOperator Operator { get; }
        ref IExpressionSyntax RightOperand { get; }
    }
}
