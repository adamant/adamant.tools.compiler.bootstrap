using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public static class TokenStreamExtensions
    {
        [MustUseReturnValue]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CurrentIs(this ITokenStream tokens, TokenKind kind)
        {
            return tokens.Current?.Kind == kind;
        }

        [MustUseReturnValue]
        public static SimpleToken? AcceptSimple(this ITokenStream tokens, TokenKind kind)
        {
            if (tokens.Current?.Kind != kind) return null;

            var token = (SimpleToken)tokens.Current;
            tokens.Next();
            return token;
        }

        [MustUseReturnValue]
        public static SimpleToken ExpectSimple(this ITokenStream tokens, TokenKind kind)
        {
            if (tokens.Current?.Kind != kind) return (SimpleToken)tokens.MissingToken();

            var token = (SimpleToken)tokens.Current;
            tokens.Next();
            return token;
        }

        [MustUseReturnValue]
        public static SimpleToken ExpectSimple(this ITokenStream tokens)
        {
            if (tokens.Current?.Kind.HasValue() ?? true)
                return (SimpleToken)tokens.MissingToken();

            var token = (SimpleToken)tokens.Current;
            tokens.Next();
            return token;
        }

        [MustUseReturnValue]
        public static StringLiteralToken ExpectStringLiteral(this ITokenStream tokens)
        {
            if (tokens.Current?.Kind != TokenKind.StringLiteral)
                return (StringLiteralToken)tokens.MissingToken();

            var token = (StringLiteralToken)tokens.Current;
            tokens.Next();
            return token;
        }

        [MustUseReturnValue]
        public static IdentifierToken ExpectIdentifier(this ITokenStream tokens)
        {
            if (!tokens.Current?.Kind.IsIdentifier() ?? true)
                return (IdentifierToken)tokens.MissingToken();

            var token = (IdentifierToken)tokens.Current;
            tokens.Next();
            return token;
        }

        [MustUseReturnValue]
        public static EndOfFileToken ExpectEndOfFile(this ITokenStream tokens)
        {
            if (tokens.Current?.Kind != TokenKind.EndOfFile)
                return (EndOfFileToken)tokens.MissingToken();

            var token = (EndOfFileToken)tokens.Current;
            tokens.Next();
            return token;
        }

        [MustUseReturnValue]
        public static bool AtEndOfFile(this ITokenStream tokens)
        {
            return tokens.Current?.Kind == TokenKind.EndOfFile;
        }

        [MustUseReturnValue]
        public static Token MissingToken(this ITokenStream tokens)
        {
            var start = tokens.Current?.Span.Start ?? tokens.File.Code.Length;
            return new Token(TokenKind.Missing, new TextSpan(start, 0));
        }
    }
}
