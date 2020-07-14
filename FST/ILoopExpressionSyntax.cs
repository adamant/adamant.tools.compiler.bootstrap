namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface ILoopExpressionSyntax : IExpressionSyntax
    {
        IBlockExpressionSyntax Block { get; }
    }
}
