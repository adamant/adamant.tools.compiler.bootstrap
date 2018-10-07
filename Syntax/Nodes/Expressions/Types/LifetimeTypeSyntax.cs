using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types
{
    public class LifetimeTypeSyntax : TypeSyntax
    {
        public NameSyntax TypeName { get; }
        public SimpleToken Dollar { get; }
        public Token Lifetime { get; }

        public LifetimeTypeSyntax(NameSyntax typeName, in SimpleToken dollar, in Token lifetime)
        {
            TypeName = typeName;
            Dollar = dollar;
            Lifetime = lifetime;
        }
    }
}
