using System.Diagnostics.Contracts;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    /// <summary>
    /// A token without a value
    /// </summary>
    public readonly struct SimpleToken : IToken
    {
        public readonly TokenKind Kind;
        public bool IsMissing => Kind == TokenKind.Missing;
        public readonly TextSpan Span;

        public SimpleToken(TokenKind kind, TextSpan span)
        {
            Requires.That(nameof(kind), !kind.HasValue());
            Kind = kind;
            Span = span;
        }

        TokenKind IToken.Kind => Kind;
        TextSpan IToken.Span => Span;
        object IToken.Value => null;

        [Pure]
        public string Text(CodeText code)
        {
            return Span.GetText(code.Text);
        }

        public static explicit operator SimpleToken(Token token)
        {
            Requires.Null(nameof(token.Value), token.Value);
            return new SimpleToken(token.Kind, token.Span);
        }

        public static implicit operator Token(SimpleToken token)
        {
            return new Token(token.Kind, token.Span);
        }
    }
}
