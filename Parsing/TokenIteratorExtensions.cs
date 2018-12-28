using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    /// <summary>
    /// * Required: If the current token is of the given type consume it, otherwise leave it, but
    ///   add a compiler error that an expected token is missing AND throw a <see cref="ParseFailedException"/>.
    /// * Expected: If the current token is of the given type consume it, otherwise leave it, but
    ///   add a compiler error that an expected token is missing.
    /// * Accept: If the current token is of the given type consume it, otherwise leave it.
    /// </summary>
    public static class TokenIteratorExtensions
    {
        #region Required
        public static TextSpan Required<T>(this ITokenIterator tokens)
            where T : IToken
        {
            if (tokens.Current is T token)
            {
                tokens.Next();
                return token.Span;
            }

            tokens.Context.Diagnostics.Add(
                ParseError.MissingToken(tokens.Context.File, typeof(T), tokens.Current));
            throw new ParseFailedException($"Requires {typeof(T).GetFriendlyName()}, found {tokens.Current.GetType().GetFriendlyName()}");
        }

        public static T RequiredToken<T>(this ITokenIterator tokens)
            where T : IToken
        {
            if (tokens.Current is T token)
            {
                tokens.Next();
                return token;
            }

            tokens.Context.Diagnostics.Add(
                ParseError.MissingToken(tokens.Context.File, typeof(T), tokens.Current));
            throw new ParseFailedException($"Requires {typeof(T).GetFriendlyName()}, found {tokens.Current.GetType().GetFriendlyName()}");
        }
        #endregion

        #region Accept

        public static bool Accept<T>(this ITokenIterator tokens)
            where T : class, IToken
        {
            if (tokens.Current is T)
            {
                tokens.Next();
                return true;
            }

            return false;
        }

        public static T AcceptToken<T>(this ITokenIterator tokens)
            where T : class, IToken
        {
            if (tokens.Current is T token)
            {
                tokens.Next();
                return token;
            }

            return null;
        }
        #endregion

        #region Expect
        public static TextSpan Expect<T>(this ITokenIterator tokens)
            where T : IToken
        {
            if (tokens.Current is T token)
            {
                tokens.Next();
                return token.Span;
            }

            tokens.Context.Diagnostics.Add(
                ParseError.MissingToken(tokens.Context.File, typeof(T), tokens.Current));
            // An empty span at the current location
            return new TextSpan(tokens.Current.Span.Start, 0);
        }

        public static (TextSpan, IIdentifierToken) ExpectIdentifier(this ITokenIterator tokens)
        {
            if (tokens.Current is IIdentifierToken identifier)
            {
                tokens.Next();
                return (identifier.Span, identifier);
            }

            tokens.Context.Diagnostics.Add(
                ParseError.MissingToken(tokens.Context.File, typeof(IIdentifierToken), tokens.Current));
            return (new TextSpan(tokens.Current.Span.Start, 0), null);
        }
        #endregion

        /// <summary>
        /// The current token is unexpected, report an error and consume it.
        /// </summary>
        public static TextSpan UnexpectedToken(this ITokenIterator tokens)
        {
            // TODO shouldn't we ignore or combine unexpected token errors until we parse something successfully?
            var span = tokens.Current.Span;
            tokens.Context.Diagnostics.Add(ParseError.UnexpectedToken(tokens.Context.File, span));
            tokens.Next();
            return span;
        }

        public static bool AtEnd<T>(this ITokenIterator tokens)
            where T : IToken
        {
            switch (tokens.Current)
            {
                case T _:
                case IEndOfFileToken _:
                    return true;
                default:
                    return false;
            }
        }
    }
}
