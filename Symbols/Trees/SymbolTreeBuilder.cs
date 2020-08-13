using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Trees
{
    /// <summary>
    /// Builder for a <see cref="ISymbolTree"/>
    /// </summary>
    public class SymbolTreeBuilder : ISymbolTree
    {
        public PackageSymbol? Package { get; }
        private readonly IDictionary<Symbol, ISet<Symbol>> symbolChildren = new Dictionary<Symbol, ISet<Symbol>>();
        public IEnumerable<Symbol> Symbols => symbolChildren.Keys;

        public SymbolTreeBuilder()
        {
            Package = null;
        }

        public SymbolTreeBuilder(PackageSymbol package)
        {
            Package = package;
            symbolChildren.Add(package, new HashSet<Symbol>());
        }

        public bool Contains(Symbol symbol)
        {
            return symbolChildren.ContainsKey(symbol);
        }

        public IEnumerable<Symbol> Children(Symbol symbol)
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
                if (!(symbol.ContainingSymbol is null))
                    GetOrAdd(symbol.ContainingSymbol).Add(symbol);
                children = new HashSet<Symbol>();
                symbolChildren.Add(symbol, children);
            }
            return children;
        }

        public FixedSymbolTree Build()
        {
            if (Package is null)
                throw new InvalidOperationException($"Can't build {nameof(FixedSymbolTree)} without a package");
            return new FixedSymbolTree(Package, symbolChildren.ToFixedDictionary(e => e.Key, e => e.Value.ToFixedSet()));
        }

        public PrimitiveSymbolTree BuildPrimitives()
        {
            if (!(Package is null))
                throw new InvalidOperationException($"Can't build {nameof(PrimitiveSymbolTree)} WITH a package");
            return new PrimitiveSymbolTree(symbolChildren.ToFixedDictionary(e => e.Key, e => e.Value.ToFixedSet()));
        }
    }
}
