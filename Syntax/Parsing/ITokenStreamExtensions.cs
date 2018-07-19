using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public static class ITokenStreamExtensions
    {
        public static bool Next(this ITokenStream tokens)
        {
            if (!tokens.Finished)
                tokens.Consume();

            return tokens.Finished;
        }

        public static bool CurrentIs(this ITokenStream tokens, TokenKind kind)
        {
            return !tokens.Finished && tokens.Current.Kind == kind;
        }

        public static Token Accept(this ITokenStream tokens, TokenKind kind)
        {
            if (!tokens.Finished && tokens.Current.Kind == kind)
                return tokens.Consume();

            return null;
        }

        public static Token Expect(this ITokenStream tokens, TokenKind kind)
        {
            if (tokens.Finished || tokens.Current.Kind != kind)
                return tokens.MissingToken(kind);
            else
                return tokens.Consume();
        }

        public static bool AtEndOfFile(this ITokenStream tokens)
        {
            return tokens.Finished || tokens.Current.Kind == TokenKind.EndOfFile;
        }

        public static Token MissingToken(this ITokenStream tokens, TokenKind kind)
        {
            if (tokens.Finished)
                return Token.Missing(tokens.Code, tokens.Code.Length, kind);
            else
                return Token.Missing(tokens.Code, tokens.Current.Span.Start, kind);
        }
    }
}
