namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IResultStatementSyntax : IStatementSyntax, IBlockOrResultSyntax
    {
        ExpressionSyntax Expression { get; }
        ref ExpressionSyntax ExpressionRef { get; }
    }
}
