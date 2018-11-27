using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class LifetimeNameSyntax : ExpressionSyntax
    {
        [NotNull] public SimpleName Name { get; }

        public LifetimeNameSyntax(TextSpan span, [NotNull] SimpleName name)
            : base(span)
        {
            Name = name;
        }

        public override string ToString()
        {
            return "$" + Name;
        }
    }
}
