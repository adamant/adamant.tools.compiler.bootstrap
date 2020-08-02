using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(IBlockOrResultSyntax),
        typeof(IIfExpressionSyntax))]
    public partial interface IElseClauseSyntax : ISyntax
    {
    }
}
