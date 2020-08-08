using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Trees
{
    /// <summary>
    /// Builder for a <see cref="SymbolTree"/>
    /// </summary>
    public class SymbolTreeBuilder : SymbolTree
    {
        private readonly IDictionary<Symbol, ISet<Symbol>> symbolChildren = new Dictionary<Symbol, ISet<Symbol>>();
        public override IEnumerable<Symbol> Symbols => symbolChildren.Keys;

        public SymbolTreeBuilder(PackageSymbol package)
            : base(package)
        {
            symbolChildren.Add(package, new HashSet<Symbol>());
        }

        public override IEnumerable<Symbol> Children(Symbol symbol)
        {
            if (symbol.Package != Package)
                throw new ArgumentException("Symbol must be for the package of this tree", nameof(symbol));

            return symbolChildren.TryGetValue(symbol, out var children)
                ? children : Enumerable.Empty<Symbol>();
        }

        public void Add(Symbol symbol)
        {
            if (symbol.Package != Package)
                throw new ArgumentException("Symbol must be for the package of this tree", nameof(symbol));

            GetOrAdd(symbol);
        }

        private ISet<Symbol> GetOrAdd(Symbol symbol)
        {
            if (!symbolChildren.TryGetValue(symbol, out var children))
            {
                // Add to parent's children
                GetOrAdd(symbol.ContainingSymbol!).Add(symbol);
                children = new HashSet<Symbol>();
                symbolChildren.Add(symbol, children);
            }
            return children;
        }

        public FixedSymbolTree Build()
        {
            return new FixedSymbolTree(Package, symbolChildren.ToFixedDictionary(e => e.Key, e => e.Value.ToFixedSet()));
        }
    }
}
