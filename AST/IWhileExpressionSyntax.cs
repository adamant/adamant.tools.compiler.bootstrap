namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IWhileExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Condition { get; }
        IBlockExpressionSyntax Block { get; }
    }
}
