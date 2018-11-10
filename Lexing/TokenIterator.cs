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

        public TokenIterator([NotNull] ParseContext context, [NotNull] IEnumerable<IToken> tokens)
        {
            Context = context.NotNull();
            this.tokens = tokens.GetEnumerator().NotNull();
            Next();
        }

        public bool Next()
        {
            if (tokens == null) return false;
            if (!tokens.MoveNext())
                tokens = null;
            return tokens != null;
        }

        [CanBeNull]
        public IToken Current => tokens?.Current;
    }
}
