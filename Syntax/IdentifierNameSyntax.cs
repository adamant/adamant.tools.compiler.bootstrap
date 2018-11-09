using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class IdentifierNameSyntax : SimpleNameSyntax
    {
        public IdentifierNameSyntax([NotNull] IIdentifierToken name)
            : base(name, name.Span)
        {
        }
    }
}
