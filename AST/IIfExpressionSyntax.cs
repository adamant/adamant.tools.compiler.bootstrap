namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IIfExpressionSyntax : IExpressionSyntax, IElseClauseSyntax
    {
        ref IExpressionSyntax Condition { get; }
        IBlockOrResultSyntax ThenBlock { get; }
        IElseClauseSyntax? ElseClause { get; }
    }
}
