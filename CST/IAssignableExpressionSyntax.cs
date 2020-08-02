using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(INameExpressionSyntax),
        typeof(IFieldAccessExpressionSyntax))]
    public partial interface IAssignableExpressionSyntax : IExpressionSyntax
    {
    }
}
