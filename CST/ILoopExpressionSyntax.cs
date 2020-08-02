namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface ILoopExpressionSyntax : IExpressionSyntax
    {
        IBlockExpressionSyntax Block { get; }
    }
}
