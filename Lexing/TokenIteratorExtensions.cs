using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Lexing
{
    public static class TokenIteratorExtensions
    {
        [NotNull]
        public static ITokenIterator WhereNotTrivia([NotNull] this ITokenIterator tokens)
        {
            return new WhereNotTriviaIterator(tokens.NotNull());
        }

        private class WhereNotTriviaIterator : ITokenIterator
        {
            [NotNull] private readonly ITokenIterator tokens;

            public WhereNotTriviaIterator([NotNull] ITokenIterator tokens)
            {
                this.tokens = tokens.NotNull();
                if (Current is ITriviaToken)
                    Next();
            }

            [NotNull] public ParseContext Context => tokens.Context;
            public bool Next()
            {
                do
                {
                    tokens.Next();
                } while (tokens.Current is ITriviaToken);
                return tokens.Current != null;
            }

            [CanBeNull]
            public IToken Current => tokens.Current;
        }
    }
}
