using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
{
    public abstract class LexicalScope
    {
        private readonly SymbolSet symbols;
        private readonly SymbolSet nestedSymbols;

        protected LexicalScope(SymbolSet symbols, SymbolSet nestedSymbols)
        {
            this.symbols = symbols;
            this.nestedSymbols = nestedSymbols;
        }

        public abstract FixedList<ISymbol> LookupInGlobalScope(Name name);

        public virtual FixedList<ISymbol> Lookup(SimpleName name, bool includeNested = true)
        {
            return symbols.TryGetValue(name, out var declaration)
                ? declaration
                : (includeNested && nestedSymbols.TryGetValue(name, out var nestedDeclaration) ? nestedDeclaration : FixedList<ISymbol>.Empty);
        }

        public FixedList<ISymbol> Lookup(Name name, bool includeNested = true)
        {
            switch (name)
            {
                default:
                    throw ExhaustiveMatch.Failed(name);
                case SimpleName simpleName:
                    return Lookup(simpleName, includeNested);
                case QualifiedName qualifiedName:
                    var containingSymbols = Lookup(qualifiedName.Qualifier, includeNested);
                    return containingSymbols.OfType<IParentSymbol>()
                        .SelectMany(s => s.ChildSymbols[qualifiedName.UnqualifiedName])
                        .ToFixedList();
            }
        }
    }
}
