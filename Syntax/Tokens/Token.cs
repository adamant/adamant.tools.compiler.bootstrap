using System.Diagnostics.Contracts;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    /// For maximum performance and minimum memory footprint, tokens are structs
    /// with as few and as small of data fields as possible. This struct can
    /// be any token whether it has a value or not.
    public readonly struct Token : IToken
    {
        public readonly TokenKind Kind;
        public bool IsMissing => Kind == TokenKind.Missing;
        public readonly TextSpan Span;
        public readonly object Value;

        public Token(TokenKind kind, TextSpan span, object value = null)
        {
            Kind = kind;
            Span = span;
            Value = value;
        }

        TokenKind IToken.Kind => Kind;
        TextSpan IToken.Span => Span;
        object IToken.Value => Value;

        [Pure]
        public string Text(CodeText code)
        {
            return Span.GetText(code.Text);
        }
    }
}
