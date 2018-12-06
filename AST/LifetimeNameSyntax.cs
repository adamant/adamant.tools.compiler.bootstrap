using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class LifetimeNameSyntax : ExpressionSyntax
    {
        public SimpleName Name { get; }

        public LifetimeNameSyntax(TextSpan span, SimpleName name)
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
