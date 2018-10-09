using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes
{
    public class FakeTokenStream : ITokenStream
    {
        [NotNull]
        public CodeFile File { get; }

        [NotNull]
        public IReadOnlyList<Token> Tokens { get; }

        [NotNull]
        private readonly TokenStream stream;

        public FakeTokenStream([NotNull] CodeFile file, [NotNull][ItemNotNull] IEnumerable<Token> tokens)
        {
            File = file;
            Tokens = tokens.ToList().AsReadOnly();
            stream = new TokenStream(file, Tokens);
        }

        public Token this[int index] => Tokens[index];

        public bool MoveNext()
        {
            return stream.MoveNext();
        }

        public void Reset()
        {
            stream.Reset();
        }

        public Token Current => stream.Current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            stream.Dispose();
        }
    }
}
