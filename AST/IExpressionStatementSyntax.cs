namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IExpressionStatementSyntax : IBodyStatementSyntax
    {
        ref IExpressionSyntax Expression { get; }
    }
}
