namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IReturnExpressionSyntax : IExpressionSyntax
    {
        IExpressionSyntax ReturnValue { get; }
        ref IExpressionSyntax ReturnValueRef { get; }
    }
}
