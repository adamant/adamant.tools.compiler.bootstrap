using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types
{
    public class LifetimeTypeSyntax : TypeSyntax
    {
        public readonly NameSyntax TypeName;
        public readonly IdentifierToken Lifetime;

        public LifetimeTypeSyntax(IEnumerable<SyntaxNode> children)
        {
            TypeName = children.OfType<NameSyntax>().Single();
            Lifetime = children.OfType<IdentifierToken>().Last();
        }
    }
}
