using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using JetBrains.Annotations;

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
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public static partial class TokenFactory
    {
        [NotNull]
        public static IIntegerLiteralToken IntegerLiteral(TextSpan span, BigInteger value)
        {
            return new IntegerLiteralToken(span, value);
        }
    }
}
