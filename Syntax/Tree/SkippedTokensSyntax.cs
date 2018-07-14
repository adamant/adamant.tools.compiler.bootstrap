using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Tree
{
    public class SkippedTokensSyntax : SyntaxNode
    {
        public SkippedTokensSyntax(Token token)
            : base(token.Yield())
        {
            // TODO add a diagnostic error
        }
    }
}
