namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface ILoopExpressionSyntax : IExpressionSyntax
    {
        BlockSyntax Block { get; }
    }
}
