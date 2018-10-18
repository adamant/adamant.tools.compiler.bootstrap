using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names
{
    public class IdentifierNameSyntax : NameSyntax
    {
        [NotNull] public IIdentifierToken Name { get; }

        public IdentifierNameSyntax([NotNull] IIdentifierToken name)
            : base(name.Span)
        {
            Requires.NotNull(nameof(name), name);
            Name = name;
        }
    }
}
