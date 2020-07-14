namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IAssignmentExpressionSyntax : IExpressionSyntax
    {
        ref IAssignableExpressionSyntax LeftOperand { get; }
        AssignmentOperator Operator { get; }
        ref IExpressionSyntax RightOperand { get; }
    }
}
