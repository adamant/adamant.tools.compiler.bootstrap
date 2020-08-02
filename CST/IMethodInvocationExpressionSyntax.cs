namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IMethodInvocationExpressionSyntax
    {
        ref IExpressionSyntax ContextExpression { get; }
    }
}
