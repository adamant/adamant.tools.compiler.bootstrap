using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing
{
    public class TokenStream : ITokenStream
    {
        [NotNull]
        public CodeFile File { get; }

        [NotNull]
        [ItemNotNull]
        private readonly IEnumerator<Token> tokens;

        public TokenStream([NotNull] CodeFile file, [NotNull][ItemNotNull] IEnumerable<Token> tokens)
        {
            Requires.NotNull(nameof(file), file);
            Requires.NotNull(nameof(tokens), tokens);
            File = file;
            this.tokens = tokens.Where(t => !(t is TriviaToken)).GetEnumerator();
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

        public Token Current => tokens.Current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            tokens.Dispose();
        }
    }
}
