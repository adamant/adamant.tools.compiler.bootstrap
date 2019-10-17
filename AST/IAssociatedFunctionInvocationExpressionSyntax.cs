namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IAssociatedFunctionInvocationExpressionSyntax : IInvocationExpressionSyntax
    {
        ICallableNameSyntax FunctionNameSyntax { get; }
    }
}
