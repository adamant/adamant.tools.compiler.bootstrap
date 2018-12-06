using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

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
        public static TextSpan Required<T>([NotNull] this ITokenIterator tokens)
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

        [NotNull]
        public static T RequiredToken<T>([NotNull] this ITokenIterator tokens)
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
        [MustUseReturnValue]
        public static bool Accept<T>([NotNull] this ITokenIterator tokens)
            where T : class, IToken
        {
            if (tokens.Current is T)
            {
                tokens.Next();
                return true;
            }

            return false;
        }

        [MustUseReturnValue]
        [CanBeNull]
        public static T AcceptToken<T>([NotNull] this ITokenIterator tokens)
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
        public static TextSpan Expect<T>([NotNull] this ITokenIterator tokens)
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

        [MustUseReturnValue]
        [CanBeNull]
        public static IIdentifierToken ExpectIdentifier([NotNull] this ITokenIterator tokens)
        {
            if (tokens.Current is IIdentifierToken identifier)
            {
                tokens.Next();
                return identifier;
            }

            tokens.Context.Diagnostics.Add(
                ParseError.MissingToken(tokens.Context.File, typeof(IIdentifierToken), tokens.Current));
            return null;
        }
        #endregion

        /// <summary>
        /// The current token is unexpected, report an error and consume it.
        /// </summary>
        public static TextSpan UnexpectedToken([NotNull] this ITokenIterator tokens)
        {
            // TODO shouldn't we ignore or combine unexpected token errors until we parse something successfully?
            var span = tokens.Current.Span;
            tokens.Context.Diagnostics.Add(ParseError.UnexpectedToken(tokens.Context.File, span));
            tokens.Next();
            return span;
        }

        [MustUseReturnValue]
        public static bool AtEnd<T>([NotNull] this ITokenIterator tokens)
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
