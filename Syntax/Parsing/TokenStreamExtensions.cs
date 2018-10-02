using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Core.Syntax;
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
            return !tokens.Finished && tokens.Current.Kind == kind;
        }

        [MustUseReturnValue]
        public static Token Accept(this ITokenStream tokens, TokenKind kind)
        {
            if (tokens.CurrentIs(kind))
                return tokens.Consume();

            return null;
        }

        [MustUseReturnValue]
        public static Token Expect(this ITokenStream tokens, TokenKind kind)
        {
            if (tokens.CurrentIs(kind))
                return tokens.Consume();

            return tokens.MissingToken(kind);
        }

        [MustUseReturnValue]
        public static IdentifierToken ExpectIdentifier(this ITokenStream tokens)
        {
            if (tokens.CurrentIs(TokenKind.Identifier))
                return (IdentifierToken)tokens.Consume();

            return (IdentifierToken)tokens.MissingToken(TokenKind.Identifier);
        }

        [MustUseReturnValue]
        public static bool AtEndOfFile(this ITokenStream tokens)
        {
            return tokens.Finished || tokens.Current.Kind == TokenKind.EndOfFile;
        }

        [MustUseReturnValue]
        public static Token MissingToken(this ITokenStream tokens, TokenKind kind)
        {
            return Token.Missing(tokens.File, tokens.Finished ? tokens.File.Code.Length : tokens.Current.Span.Start, kind);
        }
    }
}
