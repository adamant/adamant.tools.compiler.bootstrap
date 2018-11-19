using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class IdentifierNameSyntax : SimpleNameSyntax
    {
        public IdentifierNameSyntax(TextSpan span, [NotNull] string name)
            : base(new SimpleName(name), span)
        {
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
