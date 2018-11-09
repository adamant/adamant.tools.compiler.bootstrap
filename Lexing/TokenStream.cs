using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Lexing
{
    public class TokenStream : ITokenStream
    {
        [NotNull]
        public CodeFile File { get; }

        [NotNull]
        [ItemNotNull]
        private readonly IEnumerator<IToken> tokens;

        public TokenStream([NotNull] CodeFile file, [NotNull][ItemNotNull] IEnumerable<IToken> tokens)
        {
            Requires.NotNull(nameof(file), file);
            Requires.NotNull(nameof(tokens), tokens);
            File = file;
            this.tokens = tokens.Where(t => !(t is ITriviaToken)).GetEnumerator().AssertNotNull();
            this.tokens.MoveNext();
        }

        public bool MoveNext()
        {
            return tokens.MoveNext();
        }

        public void Reset()
        {
            tokens.Reset();
        }

        public IToken Current => tokens.Current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            tokens.Dispose();
        }
    }
}
