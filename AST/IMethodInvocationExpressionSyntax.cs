namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IMethodInvocationExpressionSyntax : IInvocationExpressionSyntax
    {
        ref IExpressionSyntax ContextExpression { get; }
        ICallableNameSyntax MethodNameSyntax { get; }
    }
}
