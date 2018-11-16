using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public abstract class SimpleNameSyntax : NameSyntax
    {
        [NotNull] public SimpleName Name { get; }

        protected SimpleNameSyntax([NotNull] SimpleName name, TextSpan span)
            : base(span)
        {
            Name = name;
        }
    }
}
