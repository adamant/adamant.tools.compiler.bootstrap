using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.LexicalScopes
{
    public class NestedScope<TSymbol> : LexicalScope<TSymbol>
    {
        public NestedScope(PackagesScope packagesScope)
            : base(packagesScope) { }

        public override IEnumerable<TSymbol> LookupInGlobalScope(TypeName name)
        {
            throw new System.NotImplementedException();
        }
    }
}
