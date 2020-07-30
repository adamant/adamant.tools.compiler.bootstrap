namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IUnsafeExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Expression { get; }
    }
}
