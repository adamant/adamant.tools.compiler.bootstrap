using System.Numerics;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    public partial interface IIntegerLiteralToken : IToken
    {
        BigInteger Value { get; }
    }
}
