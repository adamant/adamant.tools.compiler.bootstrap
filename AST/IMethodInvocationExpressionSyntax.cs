namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IMethodInvocationExpressionSyntax : IInvocationExpressionSyntax
    {
        ref IExpressionSyntax ContextExpression { get; }
        ICallableNameSyntax MethodNameSyntax { get; }
    }
}
