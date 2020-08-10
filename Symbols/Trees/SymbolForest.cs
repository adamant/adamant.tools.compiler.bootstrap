using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Trees
{
    public class SymbolForest
    {
        private readonly PrimitiveSymbolTree primitiveSymbolTree;
        private readonly FixedDictionary<PackageSymbol, FixedSymbolTree> packageTrees;
        public IEnumerable<Symbol> Symbols => packageTrees.Values.SelectMany(t => t.Symbols);

        public SymbolForest(PrimitiveSymbolTree primitiveSymbolTree, IEnumerable<FixedSymbolTree> packageTrees)
        {
            this.primitiveSymbolTree = primitiveSymbolTree;
            this.packageTrees = packageTrees.ToFixedDictionary(t => t.Package!);
        }

        public IEnumerable<Symbol> Children(Symbol symbol)
        {
            if (symbol.Package is null)
                return primitiveSymbolTree.Children(symbol);

            if (!packageTrees.TryGetValue(symbol.Package, out var tree))
                throw new ArgumentException("Symbol must be for one of the packages in this tree", nameof(symbol));

            return tree.Children(symbol);
        }
    }
}
