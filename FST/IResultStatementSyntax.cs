namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IResultStatementSyntax : IStatementSyntax, IBlockOrResultSyntax
    {
        ref IExpressionSyntax Expression { get; }
    }
}
