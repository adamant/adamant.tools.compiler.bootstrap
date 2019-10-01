namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IMethodInvocationSyntax : IInvocationSyntax
    {
        ref IExpressionSyntax Target { get; }
        INameSyntax MethodNameSyntax { get; }
    }
}
