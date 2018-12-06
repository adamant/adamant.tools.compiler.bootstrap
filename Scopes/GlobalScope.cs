using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
{
    public class GlobalScope : LexicalScope
    {
        public GlobalScope(IEnumerable<ISymbol> symbols)
            : base(symbols)
        {
        }

        public override FixedList<ISymbol> LookupGlobal(SimpleName name)
        {
            return Lookup(name);
        }
    }
}
