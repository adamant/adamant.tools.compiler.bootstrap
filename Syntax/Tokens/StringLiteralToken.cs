using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public readonly struct StringLiteralToken : IToken
    {
        public TokenKind Kind => IsMissing ? TokenKind.Missing : TokenKind.StringLiteral;
        public readonly bool IsMissing;
        public readonly TextSpan Span;
        public readonly string Value;

        private StringLiteralToken(TokenKind kind, TextSpan span, string value)
        {
            Requires.That(nameof(kind), kind == TokenKind.Missing || kind == TokenKind.StringLiteral);
            IsMissing = kind == TokenKind.Missing;
            Span = span;
            Value = value;
        }

        bool IToken.IsMissing => IsMissing;
        TextSpan IToken.Span => Span;
        object IToken.Value => Value;

        public static explicit operator StringLiteralToken(Token token)
        {
            return new StringLiteralToken(token.Kind, token.Span, (string)token.Value);
        }
    }
}
