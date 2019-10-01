using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
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
