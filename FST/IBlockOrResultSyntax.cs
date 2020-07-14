using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    [Closed(
        typeof(IBlockExpressionSyntax),
        typeof(IResultStatementSyntax))]
    public interface IBlockOrResultSyntax : IElseClauseSyntax
    {
    }
}
