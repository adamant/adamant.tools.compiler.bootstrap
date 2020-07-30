namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IWhileExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Condition { get; }
        IBlockExpressionSyntax Block { get; }
    }
}
