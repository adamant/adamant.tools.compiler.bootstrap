namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IFunctionInvocationExpressionSyntax : IInvocationExpressionSyntax
    {
        ICallableNameSyntax FunctionNameSyntax { get; }
    }
}
