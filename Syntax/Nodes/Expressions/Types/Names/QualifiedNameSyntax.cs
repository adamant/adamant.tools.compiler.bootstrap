using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names
{
    public class QualifiedNameSyntax : NameSyntax
    {
        public NameSyntax Qualifier { get; }
        public SimpleToken Dot { get; }
        public IdentifierNameSyntax Name { get; }

        public QualifiedNameSyntax(NameSyntax qualifier, SimpleToken dot, IdentifierNameSyntax name)
        {
            Qualifier = qualifier;
            Dot = dot;
            Name = name;
        }
    }
}
