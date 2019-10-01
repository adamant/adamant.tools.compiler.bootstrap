using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IBlockOrResultSyntax),
        typeof(IIfExpressionSyntax))]
    public interface IElseClauseSyntax : ISyntax
    {
    }
}
