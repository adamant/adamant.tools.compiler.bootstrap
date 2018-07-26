using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Names
{
    public class IdentifierNameSyntax : NameSyntax
    {
        // It could be a missing token, so then it wouldn't be an IdentifierToken
        public Token Identifier => (Token)Children.Single();

        public IdentifierNameSyntax(Token token)
            : base(token.Yield())
        {
        }
    }
}
