using System;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Lexing
{
    /// <summary>
    /// * Take - throws InvalidOperationException if that kind of token isn't find
    /// * Accept - returns null if that kind of token isn't find
    /// * Expect - returns MissingToken if that kind of token isn't find
    /// </summary>
    public static class TokenStreamExtensions
    {
        #region Take
        [MustUseReturnValue]
        [NotNull]
        public static T Take<T>([NotNull] this ITokenStream tokens)
            where T : IToken
        {
            if (tokens.Current is T token)
            {
                tokens.MoveNext();
                return token;
            }

            throw new InvalidOperationException($"Expected {typeof(T).GetFriendlyName()}, found {tokens.Current.GetType().GetFriendlyName()}");
        }

        [MustUseReturnValue]
        [NotNull]
        public static OperatorToken TakeOperator([NotNull] this ITokenStream tokens)
        {
            return Take<OperatorToken>(tokens);
        }

        [MustUseReturnValue]
        [NotNull]
        public static EndOfFileToken TakeEndOfFile([NotNull] this ITokenStream tokens)
        {
            return Take<EndOfFileToken>(tokens);
        }
        #endregion

        [MustUseReturnValue]
        [NotNull]
        public static T Expect<T>([NotNull] this ITokenStream tokens)
            where T : IToken
        {
            if (tokens.Current is T token)
            {
                tokens.MoveNext();
                return token;
            }

            return Missing<T>(tokens);
        }

        [MustUseReturnValue]
        [CanBeNull]
        public static T Accept<T>([NotNull] this ITokenStream tokens)
            where T : Token
        {
            if (tokens.Current is T token)
            {
                tokens.MoveNext();
                return token;
            }

            return null;
        }

        [MustUseReturnValue]
        [NotNull]
        public static IIdentifierToken ExpectIdentifier([NotNull] this ITokenStream tokens)
        {
            if (tokens.Current is IdentifierToken token)
            {
                tokens.MoveNext();
                return token;
            }

            return Missing<IIdentifierToken>(tokens);
        }

        [MustUseReturnValue]
        public static bool AtTerminator<T>([NotNull] this ITokenStream tokens)
            where T : Token
        {
            return tokens.Current is T || tokens.Current is EndOfFileToken;
        }

        [MustUseReturnValue]
        public static bool AtEndOfFile([NotNull] this ITokenStream tokens)
        {
            return tokens.Current is EndOfFileToken;
        }


        [MustUseReturnValue]
        [NotNull]
        public static T Missing<T>([NotNull] this ITokenStream tokens)
            where T : IToken
        {
            return (T)(object)Missing(tokens);
        }

        [MustUseReturnValue]
        [NotNull]
        public static MissingToken Missing([NotNull] this ITokenStream tokens)
        {
            return new MissingToken(new TextSpan(tokens.Current.Span.Start, 0));
        }
    }
}
