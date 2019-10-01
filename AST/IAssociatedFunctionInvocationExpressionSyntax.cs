namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IAssociatedFunctionInvocationExpressionSyntax : IInvocationExpressionSyntax
    {
        INameExpressionSyntax FunctionNameSyntax { get; }
    }
}
