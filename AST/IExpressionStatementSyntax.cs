namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IExpressionStatementSyntax : IStatementSyntax
    {
        ref IExpressionSyntax Expression { get; }
    }
}
