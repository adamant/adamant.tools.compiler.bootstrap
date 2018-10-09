using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names
{
    public class QualifiedNameSyntax : NameSyntax
    {
        public NameSyntax Qualifier { get; }

        [CanBeNull]
        public DotToken Dot { get; }

        public IdentifierNameSyntax Name { get; }

        public QualifiedNameSyntax(
            NameSyntax qualifier,
            [CanBeNull] DotToken dot,
            IdentifierNameSyntax name)
        {
            Qualifier = qualifier;
            Dot = dot;
            Name = name;
        }
    }
}
