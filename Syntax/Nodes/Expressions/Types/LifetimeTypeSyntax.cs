using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types
{
    public class LifetimeTypeSyntax : TypeSyntax
    {
        [NotNull]
        public NameSyntax TypeName { get; }

        [CanBeNull]
        public DollarToken Dollar { get; }

        [CanBeNull]
        public Token Lifetime { get; }

        public LifetimeTypeSyntax(
            [NotNull] NameSyntax typeName,
            [CanBeNull] in DollarToken dollar,
            [CanBeNull] in Token lifetime)
        {
            Requires.NotNull(nameof(typeName), typeName);
            TypeName = typeName;
            Dollar = dollar;
            Lifetime = lifetime;
        }
    }
}
