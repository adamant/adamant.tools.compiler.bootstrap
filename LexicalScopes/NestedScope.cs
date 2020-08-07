using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.LexicalScopes
{
    public class NestedScope<TSymbol> : LexicalScope<TSymbol>
    {
        public NestedScope(
            PackagesScope packagesScope,
            FixedDictionary<TypeName, FixedList<TSymbol>> symbolsInScope,
            FixedDictionary<TypeName, FixedList<TSymbol>> symbolsInNestedScopes)
            : base(packagesScope, symbolsInScope, symbolsInNestedScopes) { }

        public override IEnumerable<TSymbol> LookupInGlobalScope(TypeName name)
        {
            throw new System.NotImplementedException();
        }
    }
}
