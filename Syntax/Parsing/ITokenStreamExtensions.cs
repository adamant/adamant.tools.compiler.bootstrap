using System;
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

        public static Token Accept(this ITokenStream tokens, TokenKind kind)
        {
            if (!tokens.Finished && tokens.Current.Kind == kind)
                return tokens.Consume();

            return null;
        }

        public static Token Expect(this ITokenStream tokens, TokenKind kind)
        {
            if (tokens.Finished)
            {
                throw new NotImplementedException("return a missing token at end of source text");
            }
            else if (tokens.Current.Kind != kind)
            {
                throw new NotImplementedException("return missing token at current token location");
            }
            else
                return tokens.Consume();
        }

        public static bool AtEndOfFile(this ITokenStream tokens)
        {
            return tokens.Finished || tokens.Current.Kind == TokenKind.EndOfFile;
        }
    }
}
