using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types
{
    public class LifetimeTypeSyntax : TypeSyntax
    {
        [NotNull] public NameSyntax TypeName { get; }
        [NotNull] public DollarToken Dollar { get; }
        [NotNull] public IToken Lifetime { get; }

        public LifetimeTypeSyntax(
            [NotNull] NameSyntax typeName,
            [NotNull] DollarToken dollar,
            [NotNull] IToken lifetime)
        {
            Requires.NotNull(nameof(typeName), typeName);
            Requires.NotNull(nameof(dollar), dollar);
            Requires.NotNull(nameof(lifetime), lifetime);
            TypeName = typeName;
            Dollar = dollar;
            Lifetime = lifetime;
        }
    }
}
