namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IMethodInvocationExpressionSyntax : IInvocationExpressionSyntax
    {
        ref IExpressionSyntax Target { get; }
        INameExpressionSyntax MethodNameSyntax { get; }
    }
}
