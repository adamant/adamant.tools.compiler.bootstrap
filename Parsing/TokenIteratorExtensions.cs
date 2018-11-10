using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    /// <summary>
    /// * Required - throws <see cref="InvalidOperationException"/> if that kind of token isn't found
    /// * Take - throws InvalidOperationException if that kind of token isn't find
    /// * Accept - returns null if that kind of token isn't find
    /// * Expect - returns MissingToken if that kind of token isn't find
    /// </summary>
    public static class TokenIteratorExtensions
    {
        #region Required
        public static TextSpan Required<T>([NotNull] this ITokenIterator tokens)
            where T : IToken
        {
            if (!(tokens.Current is T))
                throw new ParseFailedException($"Requires {typeof(T).GetFriendlyName()}, found {tokens.Current?.GetType().NotNull().GetFriendlyName()}");

            var span = tokens.Current.Span;
            tokens.Next();
            return span;
        }
        #endregion

        #region Take
        [Obsolete("Use Consume() instead")]
        [MustUseReturnValue]
        [NotNull]
        public static T Take<T>([NotNull] this ITokenIterator tokens)
            where T : IToken
        {
            if (tokens.Current is T token)
            {
                tokens.Next();
                return token;
            }

            throw new InvalidOperationException($"Expected {typeof(T).GetFriendlyName()}, found {tokens.Current.GetType().GetFriendlyName()}");
        }

        [Obsolete("Use Consume() instead")]
        [MustUseReturnValue]
        [NotNull]
        public static IOperatorToken TakeOperator([NotNull] this ITokenIterator tokens)
        {
            return Take<IOperatorToken>(tokens);
        }
        #endregion

        public static TextSpan Expect<T>([NotNull] this ITokenIterator tokens)
            where T : IToken
        {
            if (!(tokens.Current is T))
                tokens.Context.Diagnostics.Add(ParseError.MissingToken(tokens.Context.File, typeof(T), tokens.Current.NotNull()));

            var span = tokens.Current.Span;
            tokens.Next();
            return span;
        }

        [MustUseReturnValue]
        [NotNull]
        public static T Consume<T>([NotNull] this ITokenIterator tokens)
            where T : ITokenPlace
        {
            if (tokens.Current is T token)
            {
                tokens.Next();
                return token;
            }

            return Missing<T>(tokens);
        }

        [MustUseReturnValue]
        [CanBeNull]
        public static T Accept<T>([NotNull] this ITokenIterator tokens)
            where T : class, IToken
        {
            if (tokens.Current is T token)
            {
                tokens.Next();
                return token;
            }

            return null;
        }

        [MustUseReturnValue]
        [NotNull]
        public static IIdentifierTokenPlace ExpectIdentifier([NotNull] this ITokenIterator tokens)
        {
            if (tokens.Current is IIdentifierToken token)
            {
                tokens.Next();
                return token;
            }

            return Missing<IIdentifierTokenPlace>(tokens);
        }

        [MustUseReturnValue]
        public static bool AtTerminator<T>([NotNull] this ITokenIterator tokens)
            where T : IToken
        {
            return tokens.Current is T || tokens.Current is IEndOfFileToken;
        }

        [MustUseReturnValue]
        public static bool AtEndOfFile([NotNull] this ITokenIterator tokens)
        {
            return tokens.Current is IEndOfFileToken;
        }

        [MustUseReturnValue]
        [NotNull]
        public static T Missing<T>([NotNull] this ITokenIterator tokens)
            where T : ITokenPlace
        {
            return (T)Missing(tokens);
        }

        [MustUseReturnValue]
        [NotNull]
        public static IMissingToken Missing([NotNull] this ITokenIterator tokens)
        {
            return TokenFactory.Missing(new TextSpan(tokens.Current.NotNull().Span.Start, 0));
        }
    }
}
