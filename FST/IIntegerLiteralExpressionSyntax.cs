using System.Numerics;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IIntegerLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        BigInteger Value { get; }
    }
}
