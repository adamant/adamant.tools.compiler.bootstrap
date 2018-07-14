using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class TokenStream : ITokenStream
    {
        public SourceReference SourceReference { get; }
        public SourceText Source { get; }
        public bool Finished { get; private set; }
        private readonly IEnumerator<Token> tokens;
        public Token Current => Finished ? null : tokens.Current;

        public TokenStream(SourceReference sourceRef, SourceText source, IEnumerable<Token> tokens)
        {
            SourceReference = sourceRef;
            Source = source;
            this.tokens = tokens.GetEnumerator();
            Finished = this.tokens.MoveNext();
        }

        public Token Consume()
        {
            var current = Current;
            Finished = tokens.MoveNext();
            return current;
        }
    }
}
