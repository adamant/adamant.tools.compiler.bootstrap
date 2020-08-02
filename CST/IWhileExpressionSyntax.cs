namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IWhileExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Condition { get; }
        IBlockExpressionSyntax Block { get; }
    }
}
