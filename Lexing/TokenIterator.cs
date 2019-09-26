using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Lexing
{
    public class TokenIterator : ITokenIterator
    {
        public ParseContext Context { get; }
        private IEnumerator<IToken>? tokens;

        public TokenIterator(ParseContext context, IEnumerable<IToken> tokens)
        {
            Context = context;
            this.tokens = tokens.GetEnumerator();
            Next();
        }

        public bool Next()
        {
            if (tokens == null)
                return false;
            if (!tokens.MoveNext())
                tokens = null;
            return tokens != null;
        }

        public IToken Current => (tokens ?? throw new InvalidOperationException("Can't access `TokenIterator.Current` after `Next()` has returned false")).Current;
    }
}
