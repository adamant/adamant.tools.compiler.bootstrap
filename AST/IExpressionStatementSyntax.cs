namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IExpressionStatementSyntax : IBodyStatementSyntax
    {
        ref IExpressionSyntax Expression { get; }
    }
}
