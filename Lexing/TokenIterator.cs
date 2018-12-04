using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Lexing
{
    public class TokenIterator : ITokenIterator
    {
        [NotNull] public ParseContext Context { get; }
        [CanBeNull] private IEnumerator<IToken> tokens;

        public TokenIterator([NotNull] ParseContext context, [NotNull, ItemNotNull] IEnumerable<IToken> tokens)
        {
            Context = context;
            this.tokens = tokens.GetEnumerator();
            Next();
        }

        public bool Next()
        {
            if (tokens == null) return false;
            if (!tokens.MoveNext())
                tokens = null;
            return tokens != null;
        }

        [NotNull]
        public IToken Current => (tokens ?? throw new InvalidOperationException("Can't access `TokenIterator.Current` after `Next()` has returned false")).Current;
    }
}
