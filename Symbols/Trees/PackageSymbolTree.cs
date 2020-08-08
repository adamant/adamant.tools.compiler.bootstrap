using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Trees
{
    /// <summary>
    /// A symbol tree for a specific package
    /// </summary>
    public class PackageSymbolTree : SymbolTree
    {
        public PackageSymbol Package { get; }
        private readonly FixedDictionary<Symbol, FixedList<Symbol>> symbolChildren;
        public override IEnumerable<Symbol> Symbols => symbolChildren.Keys;

        public PackageSymbolTree(PackageSymbol package, FixedDictionary<Symbol, FixedList<Symbol>> symbolChildren)
        {
            if (!symbolChildren.ContainsKey(package))
                throw new ArgumentException("Children must be for this package", nameof(symbolChildren));
            this.Package = package;
            this.symbolChildren = symbolChildren;
        }

        public override IEnumerable<Symbol> Children(Symbol symbol)
        {
            if (symbol.Package != Package)
                throw new ArgumentException("Symbol must be for the package of this tree", nameof(symbol));

            return symbolChildren.TryGetValue(symbol, out var children)
                ? children : FixedList<Symbol>.Empty;
        }
    }
}
