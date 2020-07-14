using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    [Closed(
        typeof(INameExpressionSyntax),
        typeof(IFieldAccessExpressionSyntax))]
    public interface IAssignableExpressionSyntax : IExpressionSyntax
    {
    }
}
