using System.Globalization;
using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Tokens
{
    internal class IntegerLiteralToken : Token, IIntegerLiteralToken
    {
        public BigInteger Value { get; }

        public IntegerLiteralToken(TextSpan span, BigInteger value)
            : base(span)
        {
            Value = value;
        }

        // Helpful for debugging

        public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);
    }

    public static partial class TokenFactory
    {

        public static IIntegerLiteralToken IntegerLiteral(TextSpan span, BigInteger value)
        {
            return new IntegerLiteralToken(span, value);
        }
    }
}
