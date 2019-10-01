namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IExpressionStatementSyntax : IStatementSyntax
    {
        IExpressionSyntax Expression { get; }
        ref IExpressionSyntax ExpressionRef { get; }
    }
}
