using System.Numerics;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IIntegerLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        BigInteger Value { get; }
    }
}
