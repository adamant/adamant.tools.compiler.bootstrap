using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class TokenStream : ITokenStream
    {
        public CodeFile File { get; }
        public bool Finished { get; private set; }
        private readonly IEnumerator<Token> tokens;
        public Token Current => Finished ? null : tokens.Current;

        public TokenStream(CodeFile file, IEnumerable<Token> tokens)
        {
            File = file;
            this.tokens = tokens.GetEnumerator();
            Finished = !this.tokens.MoveNext();
        }

        [MustUseReturnValue]
        public Token Consume()
        {
            var current = Current;
            Finished = !tokens.MoveNext();
            return current;
        }
    }
}
