using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(INameExpressionSyntax),
        typeof(IFieldAccessExpressionSyntax))]
    public interface IAssignableExpressionSyntax : IExpressionSyntax
    {
    }
}
