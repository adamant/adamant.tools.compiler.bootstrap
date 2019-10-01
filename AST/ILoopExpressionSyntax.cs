namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface ILoopExpressionSyntax : IExpressionSyntax
    {
        IBlockExpressionSyntax Block { get; }
    }
}
