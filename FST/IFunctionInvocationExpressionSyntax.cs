namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IFunctionInvocationExpressionSyntax : IInvocationExpressionSyntax
    {
        ICallableNameSyntax FunctionNameSyntax { get; }
    }
}
