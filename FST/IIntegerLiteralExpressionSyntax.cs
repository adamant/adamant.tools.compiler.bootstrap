using System.Numerics;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IIntegerLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        BigInteger Value { get; }
    }
}
