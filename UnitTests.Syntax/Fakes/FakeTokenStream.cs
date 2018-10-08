using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.UnitTests.Fakes
{
    public class FakeTokenStream : ITokenStream
    {
        public CodeFile File { get; }
        public IReadOnlyList<Token> Tokens { get; }
        private readonly TokenStream stream;

        public FakeTokenStream(CodeFile file, IEnumerable<Token> tokens)
        {
            File = file;
            Tokens = tokens.ToList().AsReadOnly();
            stream = new TokenStream(file, Tokens);
        }

        public Token? Current => stream.Current;

        public bool Next()
        {
            return stream.Next();
        }

        public Token this[int index] => Tokens[index];
    }
}
