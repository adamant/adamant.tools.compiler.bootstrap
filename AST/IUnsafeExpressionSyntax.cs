namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IUnsafeExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Expression { get; }
    }
}
