using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IBlockSyntax),
        typeof(IResultStatementSyntax))]
    public interface IBlockOrResultSyntax : IElseClauseSyntax
    {
    }
}
