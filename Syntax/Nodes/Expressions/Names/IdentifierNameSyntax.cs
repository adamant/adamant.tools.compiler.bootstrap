using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Names
{
    public class IdentifierNameSyntax : NameSyntax
    {
        public IdentifierNameSyntax(Token token)
            : base(token.Yield())
        {
        }
    }
}
