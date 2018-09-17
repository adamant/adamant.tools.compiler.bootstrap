using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names
{
    public class QualifiedNameSyntax : NameSyntax
    {
        public NameSyntax Qualifier { get; }
        public IdentifierNameSyntax Name { get; }

        public QualifiedNameSyntax(IEnumerable<SyntaxNode> children)
            : base(children)
        {
            Qualifier = Children.OfType<NameSyntax>().First();
            Name = Children.OfType<IdentifierNameSyntax>().Last();
        }
    }
}
