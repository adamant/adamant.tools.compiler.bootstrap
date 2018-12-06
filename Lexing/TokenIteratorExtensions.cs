using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using ITriviaToken = Adamant.Tools.Compiler.Bootstrap.Tokens.ITriviaToken;

namespace Adamant.Tools.Compiler.Bootstrap.Lexing
{
    public static class TokenIteratorExtensions
    {

        public static ITokenIterator WhereNotTrivia(this ITokenIterator tokens)
        {
            return new WhereNotTriviaIterator(tokens);
        }

        private class WhereNotTriviaIterator : ITokenIterator
        {
            private readonly ITokenIterator tokens;

            public WhereNotTriviaIterator(ITokenIterator tokens)
            {
                this.tokens = tokens;
                if (Current is ITriviaToken)
                    Next();
            }

            public ParseContext Context => tokens.Context;

            public bool Next()
            {
                do
                {
                    if (!tokens.Next())
                        return false;
                } while (tokens.Current is ITriviaToken);

                return true;
            }

            public IToken Current => tokens.Current;
        }
    }
}
