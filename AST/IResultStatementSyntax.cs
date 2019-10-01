namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IResultStatementSyntax : IStatementSyntax, IBlockOrResultSyntax
    {
        ref IExpressionSyntax Expression { get; }
    }
}
