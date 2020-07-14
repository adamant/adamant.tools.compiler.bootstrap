namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IWhileExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Condition { get; }
        IBlockExpressionSyntax Block { get; }
    }
}
