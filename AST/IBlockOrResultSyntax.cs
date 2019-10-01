using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IBlockExpressionSyntax),
        typeof(IResultStatementSyntax))]
    public interface IBlockOrResultSyntax : IElseClauseSyntax
    {
    }
}
