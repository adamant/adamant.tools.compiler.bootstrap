using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class IdentifierNameSyntax : NameSyntax
    {
        public IdentifierNameSyntax(TextSpan span, SimpleName name)
            : base(name, span)
        {
        }

        public override string ToString()
        {
            return Name.ToString();
        }

        public FixedList<ISymbol> LookupInContainingScope()
        {
            return ContainingScope.Lookup(Name);
        }
    }
}
