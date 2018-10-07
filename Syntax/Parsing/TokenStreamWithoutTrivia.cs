using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    /// <summary>
    /// A token stream that filters out the syntax trivia, i.e. whitespace and comments
    /// </summary>
    public class TokenStreamWithoutTrivia : ITokenStream
    {
        private readonly ITokenStream source;

        public TokenStreamWithoutTrivia(ITokenStream source)
        {
            this.source = source;
        }

        public CodeFile File => source.File;

        public Token? Current => source.Current;

        public bool Next()
        {
            while (source.Next())
            {
                // Must be a current because next returned true
                switch (source.Current.Value.Kind)
                {
                    case TokenKind.Whitespace:
                    case TokenKind.Comment:
                        continue; // move to the next token
                    default:
                        return true; // stop on this token
                }
            }

            return false;
        }
    }
}
