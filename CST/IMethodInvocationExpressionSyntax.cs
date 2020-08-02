namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IMethodInvocationExpressionSyntax : IInvocationExpressionSyntax
    {
        ref IExpressionSyntax ContextExpression { get; }
        ICallableNameSyntax MethodNameSyntax { get; }
    }
}
