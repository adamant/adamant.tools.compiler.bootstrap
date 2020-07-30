namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IMethodInvocationExpressionSyntax : IInvocationExpressionSyntax
    {
        ref IExpressionSyntax ContextExpression { get; }
        ICallableNameSyntax MethodNameSyntax { get; }
    }
}
