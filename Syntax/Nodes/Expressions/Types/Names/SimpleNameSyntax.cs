using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.Identifiers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names
{
    public abstract class SimpleNameSyntax : NameSyntax
    {
        [NotNull] public IIdentifierToken Name { get; }

        protected SimpleNameSyntax([NotNull] IIdentifierToken name, TextSpan span)
            : base(span)
        {
            Requires.NotNull(nameof(name), name);
            Name = name;
        }
    }
}
