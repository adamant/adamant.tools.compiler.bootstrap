namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IUnsafeExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Expression { get; }
    }
}
