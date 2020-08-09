namespace Adamant.Tools.Compiler.Bootstrap.LexicalScopes
{
    //public class GlobalScope<TSymbol> : LexicalScope<TSymbol>
    //{
    //    public GlobalScope(
    //        PackagesScope packagesScope,
    //        FixedDictionary<TypeName, FixedSet<TSymbol>> symbolsInScope,
    //        FixedDictionary<TypeName, FixedSet<TSymbol>> symbolsInNestedScopes)
    //        : base(symbolsInScope, symbolsInNestedScopes) { }

    //    public override IEnumerable<TSymbol> LookupInGlobalScope(TypeName name)
    //    {
    //        // Don't include nested scopes, it must be in the global scope because it is global qualified
    //        return Lookup(name, includeNested: false);
    //    }
    //}
}
