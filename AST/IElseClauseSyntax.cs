using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IBlockOrResultSyntax),
        typeof(IfExpressionSyntax))]
    public interface IElseClauseSyntax
    {
        TextSpan Span { get; }
    }
}
