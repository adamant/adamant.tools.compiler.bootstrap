using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    public readonly struct IdentifierToken : IToken
    {
        public readonly TokenKind Kind;
        public bool IsMissing => Kind == TokenKind.Missing;
        public readonly TextSpan Span;
        public readonly string Value;

        public IdentifierToken(TokenKind kind, TextSpan span, string value)
        {
            Requires.That(nameof(kind), kind == TokenKind.Missing || kind.IsIdentifier());
            Kind = kind;
            Span = span;
            Value = value;
        }

        TokenKind IToken.Kind => Kind;
        TextSpan IToken.Span => Span;
        object IToken.Value => Value;

        public static explicit operator IdentifierToken(Token token)
        {
            return new IdentifierToken(token.Kind, token.Span, (string)token.Value);
        }

        public static implicit operator Token(IdentifierToken token)
        {
            return new Token(token.Kind, token.Span, token.Value);
        }
    }
}
