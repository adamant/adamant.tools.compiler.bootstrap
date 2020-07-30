using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
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
