using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names
{
    public class IdentifierNameSyntax : NameSyntax
    {
        public IdentifierToken Name { get; }

        public IdentifierNameSyntax(IdentifierToken name)
        {
            Name = name;
        }
    }
}
