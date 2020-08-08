using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.LexicalScopes
{
    public class NestedScope<TSymbol> : LexicalScope<TSymbol>
    {
        private readonly LexicalScope<TSymbol> containingScope;

        public NestedScope(
            LexicalScope<TSymbol> containingScope,
            FixedDictionary<TypeName, FixedSet<TSymbol>> symbolsInScope,
            FixedDictionary<TypeName, FixedSet<TSymbol>> symbolsInNestedScopes)
            : base(containingScope.PackagesScope, symbolsInScope, symbolsInNestedScopes)
        {
            this.containingScope = containingScope;
        }

        public NestedScope(
            LexicalScope<TSymbol> containingScope,
            FixedDictionary<TypeName, FixedSet<TSymbol>> symbolsInScope)
            : this(containingScope, symbolsInScope, FixedDictionary<TypeName, FixedSet<TSymbol>>.Empty)
        {
        }

        public override IEnumerable<TSymbol> LookupInGlobalScope(TypeName name)
        {
            throw new System.NotImplementedException();
        }
    }
}
