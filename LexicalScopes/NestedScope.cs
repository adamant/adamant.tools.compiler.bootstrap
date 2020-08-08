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
            FixedDictionary<TypeName, FixedList<TSymbol>> symbolsInScope,
            FixedDictionary<TypeName, FixedList<TSymbol>> symbolsInNestedScopes)
            : base(containingScope.PackagesScope, symbolsInScope, symbolsInNestedScopes)
        {
            this.containingScope = containingScope;
        }

        public NestedScope(
            LexicalScope<TSymbol> containingScope,
            FixedDictionary<TypeName, FixedList<TSymbol>> symbolsInScope)
            : this(containingScope, symbolsInScope, FixedDictionary<TypeName, FixedList<TSymbol>>.Empty)
        {
        }

        public override IEnumerable<TSymbol> LookupInGlobalScope(TypeName name)
        {
            throw new System.NotImplementedException();
        }
    }
}
