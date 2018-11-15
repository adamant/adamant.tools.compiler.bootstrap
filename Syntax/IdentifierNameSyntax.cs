using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class IdentifierNameSyntax : SimpleNameSyntax
    {
        public IdentifierNameSyntax([NotNull] IIdentifierTokenPlace name)
            : base(name, name.Span)
        {
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
