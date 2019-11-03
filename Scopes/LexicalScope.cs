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

        public virtual FixedList<ISymbol> Lookup(SimpleName name)
        {
            return symbols.TryGetValue(name, out var declaration)
                ? declaration
                : (nestedSymbols.TryGetValue(name, out var nestedDeclaration) ? nestedDeclaration : FixedList<ISymbol>.Empty);
        }

        public abstract FixedList<ISymbol> LookupGlobal(SimpleName name);

        public FixedList<ISymbol> LookupQualified(Name name)
        {
            switch (name)
            {
                default:
                    throw ExhaustiveMatch.Failed(name);
                case SimpleName simpleName:
                    return Lookup(simpleName);
                case QualifiedName qualifiedName:
                    var containingSymbols = LookupQualified(qualifiedName.Qualifier);
                    return containingSymbols.OfType<IParentSymbol>()
                        .SelectMany(s => s.ChildSymbols[qualifiedName.UnqualifiedName])
                        .ToFixedList();
            }
        }
    }
}
