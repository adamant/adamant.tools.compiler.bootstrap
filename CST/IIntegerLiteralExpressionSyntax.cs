using System.Numerics;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IIntegerLiteralExpressionSyntax : ILiteralExpressionSyntax
    {
        BigInteger Value { get; }
    }
}
