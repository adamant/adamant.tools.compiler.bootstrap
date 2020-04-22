using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Lexing
{
    public class TokenIterator<TToken> : ITokenIterator<TToken>
        where TToken : class, IToken
    {
        public ParseContext Context { get; }
        private IEnumerator<TToken>? tokens;

        public TokenIterator(ParseContext context, IEnumerable<TToken> tokens)
        {
            Context = context;
            this.tokens = tokens.GetEnumerator();
            Next();
        }

        public bool Next()
        {
            if (tokens is null)
                return false;
            if (!tokens.MoveNext())
                tokens = null;
            return tokens != null;
        }

        public TToken Current => (tokens ?? throw new InvalidOperationException("Can't access `TokenIterator.Current` after `Next()` has returned false")).Current;
    }
}
