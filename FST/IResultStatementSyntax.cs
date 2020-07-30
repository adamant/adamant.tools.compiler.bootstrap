namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IResultStatementSyntax : IStatementSyntax, IBlockOrResultSyntax
    {
        ref IExpressionSyntax Expression { get; }
    }
}
