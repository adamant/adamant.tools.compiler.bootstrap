namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IFunctionInvocationExpressionSyntax : IInvocationExpressionSyntax
    {
        INameExpressionSyntax FunctionNameSyntax { get; }
    }
}
