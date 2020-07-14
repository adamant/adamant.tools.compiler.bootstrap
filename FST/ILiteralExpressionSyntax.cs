using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    [Closed(
        typeof(INoneLiteralExpressionSyntax),
        typeof(IStringLiteralExpressionSyntax),
        typeof(IIntegerLiteralExpressionSyntax),
        typeof(IBoolLiteralExpressionSyntax))]
    public interface ILiteralExpressionSyntax : IExpressionSyntax
    {
    }
}
