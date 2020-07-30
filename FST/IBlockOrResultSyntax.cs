using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(IBlockExpressionSyntax),
        typeof(IResultStatementSyntax))]
    public interface IBlockOrResultSyntax : IElseClauseSyntax
    {
    }
}
