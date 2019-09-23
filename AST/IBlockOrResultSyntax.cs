using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(BlockSyntax),
        typeof(ResultStatementSyntax))]
    public interface IBlockOrResultSyntax : IElseClauseSyntax
    {
    }
}
