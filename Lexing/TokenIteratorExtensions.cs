using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using ITriviaToken = Adamant.Tools.Compiler.Bootstrap.Tokens.ITriviaToken;

namespace Adamant.Tools.Compiler.Bootstrap.Lexing
{
    public static class TokenIteratorExtensions
    {
        public static ITokenIterator<IEssentialToken> WhereNotTrivia(this ITokenIterator<IToken> tokens)
        {
            return new WhereNotTriviaIterator(tokens);
        }

        private class WhereNotTriviaIterator : ITokenIterator<IEssentialToken>
        {
            private readonly ITokenIterator<IToken> tokens;

            public WhereNotTriviaIterator(ITokenIterator<IToken> tokens)
            {
                this.tokens = tokens;
                if (tokens.Current is ITriviaToken)
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

            public IEssentialToken Current => (IEssentialToken)tokens.Current;
        }
    }
}
