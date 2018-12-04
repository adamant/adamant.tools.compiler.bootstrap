using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;
using ITriviaToken = Adamant.Tools.Compiler.Bootstrap.Tokens.ITriviaToken;

namespace Adamant.Tools.Compiler.Bootstrap.Lexing
{
    public static class TokenIteratorExtensions
    {
        [NotNull]
        public static ITokenIterator WhereNotTrivia([NotNull] this ITokenIterator tokens)
        {
            return new WhereNotTriviaIterator(tokens);
        }

        private class WhereNotTriviaIterator : ITokenIterator
        {
            [NotNull] private readonly ITokenIterator tokens;

            public WhereNotTriviaIterator([NotNull] ITokenIterator tokens)
            {
                this.tokens = tokens;
                if (Current is ITriviaToken)
                    Next();
            }

            [NotNull] public ParseContext Context => tokens.Context;

            public bool Next()
            {
                do
                {
                    if (!tokens.Next())
                        return false;
                } while (tokens.Current is ITriviaToken);

                return true;
            }

            [NotNull]
            public IToken Current => tokens.Current;
        }
    }
}
