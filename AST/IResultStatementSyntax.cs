namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IResultStatementSyntax : IStatementSyntax, IBlockOrResultSyntax
    {
        IExpressionSyntax Expression { get; }
        ref IExpressionSyntax ExpressionRef { get; }
    }
}
