using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
{
    public class NestedScope : LexicalScope
    {
        public LexicalScope ContainingScope { get; }

        public NestedScope(
            LexicalScope containingScope,
            IEnumerable<ISymbol> symbols)
            : base(symbols)
        {
            ContainingScope = containingScope;
        }

        public override FixedList<ISymbol> Lookup(SimpleName name)
        {
            var symbols = base.Lookup(name);
            return symbols.Any() ? symbols : ContainingScope.Lookup(name);
        }

        public override FixedList<ISymbol> LookupGlobal(SimpleName name)
        {
            return ContainingScope.LookupGlobal(name);
        }
    }
}
