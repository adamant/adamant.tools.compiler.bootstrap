using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.LexicalScopes
{
    public class GlobalScope<TSymbol> : LexicalScope<TSymbol>
    {
        public GlobalScope(
            PackagesScope packagesScope,
            FixedDictionary<TypeName, FixedList<TSymbol>> symbolsInScope,
            FixedDictionary<TypeName, FixedList<TSymbol>> symbolsInNestedScopes)
            : base(packagesScope, symbolsInScope, symbolsInNestedScopes) { }

        public override IEnumerable<TSymbol> LookupInGlobalScope(TypeName name)
        {
            // Don't include nested scopes, it must be in the global scope because it is global qualified
            return Lookup(name, includeNested: false);
        }
    }
}
