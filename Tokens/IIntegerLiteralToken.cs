using System.Numerics;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public interface IIntegerLiteralToken : IToken
    {
        BigInteger Value { get; }
    }
}
