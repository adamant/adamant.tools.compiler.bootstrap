using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class SkippedTokensSyntax : SyntaxBranchNode
    {
        public SkippedTokensSyntax(Token token)
            : base(token.Yield())
        {
            // TODO add a diagnostic error
        }
    }
}
