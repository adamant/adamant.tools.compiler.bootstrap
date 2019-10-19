using System.Numerics;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public partial interface IIntegerLiteralToken : ILiteralToken
    {
        BigInteger Value { get; }
    }
}
