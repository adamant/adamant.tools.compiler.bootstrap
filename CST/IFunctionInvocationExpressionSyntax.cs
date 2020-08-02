namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IFunctionInvocationExpressionSyntax : IInvocationExpressionSyntax
    {
        ICallableNameSyntax FunctionNameSyntax { get; }
    }
}
