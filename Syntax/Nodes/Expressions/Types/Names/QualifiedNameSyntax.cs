using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names
{
    public class QualifiedNameSyntax : NameSyntax
    {
        [NotNull] public NameSyntax Qualifier { get; }
        [CanBeNull] public DotToken Dot { get; }
        [NotNull] public IdentifierNameSyntax Name { get; }

        public QualifiedNameSyntax(
            [NotNull] NameSyntax qualifier,
            [CanBeNull] DotToken dot,
            [NotNull] IdentifierNameSyntax name)
        {
            Requires.NotNull(nameof(qualifier), qualifier);
            Requires.NotNull(nameof(name), name);
            Qualifier = qualifier;
            Dot = dot;
            Name = name;
        }
    }
}
