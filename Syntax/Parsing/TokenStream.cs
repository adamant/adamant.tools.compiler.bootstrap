using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class TokenStream : ITokenStream
    {
        public CodeReference CodeReference { get; }
        public CodeText Code { get; }
        public bool Finished { get; private set; }
        private readonly IEnumerator<Token> tokens;
        public Token Current => Finished ? null : tokens.Current;

        public TokenStream(CodeReference codeRef, CodeText code, IEnumerable<Token> tokens)
        {
            CodeReference = codeRef;
            Code = code;
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
