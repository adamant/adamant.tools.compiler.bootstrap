namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IFunctionInvocationExpressionSyntax : IInvocationExpressionSyntax
    {
        ICallableNameSyntax FunctionNameSyntax { get; }
    }
}
