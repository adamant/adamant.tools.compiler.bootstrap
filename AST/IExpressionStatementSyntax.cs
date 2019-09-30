namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IExpressionStatementSyntax : IStatementSyntax
    {
        ExpressionSyntax Expression { get; }
        ref ExpressionSyntax ExpressionRef { get; }
    }
}
