using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing
{
    public static class TokenStreamExtensions
    {
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
        [CanBeNull]
        public static T Expect<T>([NotNull] this ITokenStream tokens)
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
        [CanBeNull]
        public static KeywordToken ExpectKeyword([NotNull] this ITokenStream tokens)
        {
            if (tokens.Current is KeywordToken token)
            {
                tokens.MoveNext();
                return token;
            }

            return null;
        }

        [MustUseReturnValue]
        [CanBeNull]
        public static OperatorToken ExpectOperator([NotNull] this ITokenStream tokens)
        {
            if (tokens.Current is OperatorToken token)
            {
                tokens.MoveNext();
                return token;
            }

            return null;
        }

        [MustUseReturnValue]
        [CanBeNull]
        public static StringLiteralToken ExpectStringLiteral([NotNull] this ITokenStream tokens)
        {
            if (tokens.Current is StringLiteralToken token)
            {
                tokens.MoveNext();
                return token;
            }

            return null;
        }

        [MustUseReturnValue]
        [CanBeNull]
        public static IdentifierToken ExpectIdentifier([NotNull] this ITokenStream tokens)
        {
            if (tokens.Current is IdentifierToken token)
            {
                tokens.MoveNext();
                return token;
            }

            return null;
        }

        [MustUseReturnValue]
        [CanBeNull]
        public static EndOfFileToken ExpectEndOfFile([NotNull] this ITokenStream tokens)
        {
            if (tokens.Current is EndOfFileToken token)
            {
                tokens.MoveNext();
                return token;
            }

            return null;
        }

        [MustUseReturnValue]
        public static bool AtEndOfFile([NotNull] this ITokenStream tokens)
        {
            return tokens.Current is EndOfFileToken;
        }

        [MustUseReturnValue]
        [CanBeNull]
        public static T MissingToken<T>([NotNull] this ITokenStream tokens)
            where T : Token
        {
            return null;
        }
    }
}
