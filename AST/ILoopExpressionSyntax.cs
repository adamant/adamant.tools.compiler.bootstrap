namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface ILoopExpressionSyntax : IExpressionSyntax
    {
        IBlockSyntax Block { get; }
    }
}
