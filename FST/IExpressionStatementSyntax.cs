namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IExpressionStatementSyntax : IBodyStatementSyntax
    {
        ref IExpressionSyntax Expression { get; }
    }
}
