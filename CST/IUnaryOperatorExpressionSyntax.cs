namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IUnaryOperatorExpressionSyntax
    {
        ref IExpressionSyntax Operand { get; }
    }
}
