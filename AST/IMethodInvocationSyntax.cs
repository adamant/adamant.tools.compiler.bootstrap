namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IMethodInvocationSyntax : IInvocationSyntax
    {
        ref IExpressionSyntax Target { get; }
        NameSyntax MethodNameSyntax { get; }
    }
}
