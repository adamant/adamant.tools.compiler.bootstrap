namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IResultStatementSyntax : IStatementSyntax, IBlockOrResultSyntax
    {
        ref IExpressionSyntax Expression { get; }
    }
}
