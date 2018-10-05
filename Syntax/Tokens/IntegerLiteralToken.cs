using System.Numerics;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public readonly struct IntegerLiteralToken : IToken
    {
        public TokenKind Kind => IsMissing ? TokenKind.Missing : TokenKind.StringLiteral;
        public readonly bool IsMissing;
        public readonly TextSpan Span;
        public readonly BigInteger Value;

        public IntegerLiteralToken(TokenKind kind, TextSpan span, BigInteger value)
        {
            Requires.That(nameof(kind), kind == TokenKind.Missing || kind == TokenKind.IntegerLiteral);
            IsMissing = kind == TokenKind.Missing;
            Span = span;
            Value = value;
        }

        bool IToken.IsMissing => IsMissing;
        TextSpan IToken.Span => Span;
        object IToken.Value => Value;
    }
}
