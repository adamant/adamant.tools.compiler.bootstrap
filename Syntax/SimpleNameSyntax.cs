using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public abstract class SimpleNameSyntax : NameSyntax
    {
        [NotNull] public IIdentifierTokenPlace Name { get; }

        protected SimpleNameSyntax([NotNull] IIdentifierTokenPlace name, TextSpan span)
            : base(span)
        {
            Requires.NotNull(nameof(name), name);
            Name = name;
        }
    }
}