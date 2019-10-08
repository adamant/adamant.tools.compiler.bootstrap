namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IAssignmentExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax LeftOperand { get; }
        AssignmentOperator Operator { get; }
        ITransferSyntax RightOperand { get; }
    }
}
