using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens
{
    /// For maximum performance and minimum memory footprint, tokens are structs
    /// with as few and as small of data fields as possible. This struct can
    /// be any token whether it has a value or not.
    /// Note: Only Token implemented <see cref="ISyntaxNodeOrToken"/> so there
    /// are fewer cases to deal with
    public readonly struct Token : IToken, ISyntaxNodeOrToken, IEquatable<Token>
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

        #region Equals
        public override bool Equals(object obj)
        {
            return obj is Token token && Equals(token);
        }

        public bool Equals(Token other)
        {
            return Kind == other.Kind &&
                   Span.Equals(other.Span) &&
                   EqualityComparer<object>.Default.Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Kind, Span, Value);
        }

        public static bool operator ==(Token token1, Token token2)
        {
            return token1.Equals(token2);
        }

        public static bool operator !=(Token token1, Token token2)
        {
            return !(token1 == token2);
        }
        #endregion

        public override string ToString()
        {
            return Value == null ? $"Token({Kind}, {Span})" : $"Token({Kind}, {Span}, {Value})";
        }
    }
}
