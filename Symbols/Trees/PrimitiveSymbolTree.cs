using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Trees
{
    public class PrimitiveSymbolTree : SymbolTree
    {
        private readonly FixedDictionary<Symbol, FixedSet<Symbol>> symbolChildren;
        public override IEnumerable<Symbol> Symbols => symbolChildren.Keys;

        public PrimitiveSymbolTree(FixedDictionary<Symbol, FixedSet<Symbol>> symbolChildren)
        {
            this.symbolChildren = symbolChildren;
        }

        public override IEnumerable<Symbol> Children(Symbol symbol)
        {
            if (!(symbol.Package is null))
                throw new ArgumentException("Symbol must be primitive", nameof(symbol));

            return symbolChildren.TryGetValue(symbol, out var children)
                ? children : FixedSet<Symbol>.Empty;
        }
    }
}
