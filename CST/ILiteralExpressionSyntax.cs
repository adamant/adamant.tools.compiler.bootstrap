using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(INoneLiteralExpressionSyntax),
        typeof(IStringLiteralExpressionSyntax),
        typeof(IIntegerLiteralExpressionSyntax),
        typeof(IBoolLiteralExpressionSyntax))]
    public partial interface ILiteralExpressionSyntax : IExpressionSyntax
    {
    }
}
