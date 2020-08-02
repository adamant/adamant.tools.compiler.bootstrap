namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IExpressionStatementSyntax : IBodyStatementSyntax
    {
        ref IExpressionSyntax Expression { get; }
    }
}
