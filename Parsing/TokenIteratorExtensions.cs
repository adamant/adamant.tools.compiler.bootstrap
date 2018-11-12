using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing
{
    /// <summary>
    /// * AssertAndConsume: Asserts the current token is of the given type and if it is, consumes it.
    ///   Used when there must be a programmer mistake if the current token is not of the expected kind.
    /// * Accept - If the current token is of the given type consume it, otherwise leave it.
    /// * Expected - If the current token is of the given type consume it, otherwise leave it, but
    ///   add a compiler error that an expected token is missing
    ///
    /// Old Stuff
    /// * Required - throws <see cref="InvalidOperationException"/> if that kind of token isn't found
    /// * Take - throws InvalidOperationException if that kind of token isn't find
    /// * Accept - returns null if that kind of token isn't find
    /// * Expect - returns MissingToken if that kind of token isn't find
    /// </summary>
    public static class TokenIteratorExtensions
    {
        #region AssertAndConsume
        public static TextSpan AssertAndConsume<T>([NotNull] this ITokenIterator tokens)
            where T : IToken
        {
            if (tokens.Current is T token)
            {
                tokens.Next();
                return token.Span;
            }

            throw new InvalidOperationException($"Requires {typeof(T).GetFriendlyName()}, found {tokens.Current.GetType().NotNull().GetFriendlyName()}");
        }

        [NotNull]
        public static IIdentifierToken AssertAndConsumeIdentifier([NotNull] this ITokenIterator tokens)
        {
            if (tokens.Current is IIdentifierToken identifier)
            {
                tokens.Next();
                return identifier;
            }

            throw new InvalidOperationException($"Requires identifier, found {tokens.Current.GetType().NotNull().GetFriendlyName()}");
        }
        #endregion

        #region Accept
        [MustUseReturnValue]
        public static TextSpan? Accept<T>([NotNull] this ITokenIterator tokens)
            where T : class, IToken
        {
            if (tokens.Current is T token)
            {
                tokens.Next();
                return token.Span;
            }

            return null;
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

        #region Obsolete
        [Obsolete("Use AssertAndConsume() instead")]
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

        [Obsolete("Use AssertAndConsume() instead")]
        [MustUseReturnValue]
        [NotNull]
        public static IOperatorToken TakeOperator([NotNull] this ITokenIterator tokens)
        {
            return Take<IOperatorToken>(tokens);
        }

        [Obsolete("Use Expect() instead")]
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

        [Obsolete("Avoid use of IMissingToken, use null and span instead")]
        [MustUseReturnValue]
        [NotNull]
        public static T Missing<T>([NotNull] this ITokenIterator tokens)
            where T : ITokenPlace
        {
            return (T)Missing(tokens);
        }

        [Obsolete("Avoid use of IMissingToken, use null and span instead")]
        [MustUseReturnValue]
        [NotNull]
        public static IMissingToken Missing([NotNull] this ITokenIterator tokens)
        {
            return TokenFactory.Missing(new TextSpan(tokens.Current.NotNull().Span.Start, 0));
        }
        #endregion
    }
}
