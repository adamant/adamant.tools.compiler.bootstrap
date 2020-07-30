namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface ILoopExpressionSyntax : IExpressionSyntax
    {
        IBlockExpressionSyntax Block { get; }
    }
}
