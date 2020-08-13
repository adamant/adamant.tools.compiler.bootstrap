using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Trees
{
    public sealed class PrimitiveSymbolTree : ISymbolTree
    {
        PackageSymbol? ISymbolTree.Package => null;
        public FixedSet<Symbol> GlobalSymbols { get; }
        private readonly FixedDictionary<Symbol, FixedSet<Symbol>> symbolChildren;
        public IEnumerable<Symbol> Symbols => symbolChildren.Keys;

        public PrimitiveSymbolTree(FixedDictionary<Symbol, FixedSet<Symbol>> symbolChildren)
        {
            this.symbolChildren = symbolChildren;
            GlobalSymbols = symbolChildren.Keys.Where(s => s.ContainingSymbol is null).ToFixedSet();
        }

        public bool Contains(Symbol symbol)
        {
            return symbolChildren.ContainsKey(symbol);
        }

        public IEnumerable<Symbol> Children(Symbol symbol)
        {
            if (!(symbol.Package is null))
                throw new ArgumentException("Symbol must be primitive", nameof(symbol));

            return symbolChildren.TryGetValue(symbol, out var children)
                ? children : FixedSet<Symbol>.Empty;
        }
    }
}
