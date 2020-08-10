using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Trees
{
    /// <summary>
    /// A symbol tree for a specific package
    /// </summary>
    public class FixedSymbolTree : SymbolTree
    {
        public PackageSymbol Package { get; }
        private readonly FixedDictionary<Symbol, FixedSet<Symbol>> symbolChildren;
        public override IEnumerable<Symbol> Symbols => symbolChildren.Keys;

        public FixedSymbolTree(PackageSymbol package, FixedDictionary<Symbol, FixedSet<Symbol>> symbolChildren)
        {
            if (symbolChildren.Keys.Any(s => s.Package != package))
                throw new ArgumentException("Children must be for this package", nameof(symbolChildren));
            Package = package;
            this.symbolChildren = symbolChildren;
        }

        public override IEnumerable<Symbol> Children(Symbol symbol)
        {
            if (symbol.Package != Package)
                throw new ArgumentException("Symbol must be for the package of this tree", nameof(symbol));

            return symbolChildren.TryGetValue(symbol, out var children)
                ? children : FixedSet<Symbol>.Empty;
        }
    }
}
