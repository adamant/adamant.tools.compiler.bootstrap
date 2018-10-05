using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class TokenStream : ITokenStream
    {
        public CodeFile File { get; }
        private readonly IEnumerator<Token> tokens;
        public Token? Current { get; private set; }

        public TokenStream(CodeFile file, IEnumerable<Token> tokens)
        {
            File = file;
            this.tokens = tokens.GetEnumerator();
            Current = this.tokens.MoveNext() ? this.tokens.Current : default(Token?);
        }

        [MustUseReturnValue]
        public bool Next()
        {
            var hasValue = tokens.MoveNext();
            Current = hasValue ? tokens.Current : default(Token?);
            return hasValue;
        }
    }
}
