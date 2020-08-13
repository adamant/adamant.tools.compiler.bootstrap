using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Trees
{
    public class SymbolForest
    {
        public PrimitiveSymbolTree PrimitiveSymbolTree { get; }
        private readonly FixedDictionary<PackageSymbol, ISymbolTree> packageTrees;
        public IEnumerable<PackageSymbol> Packages => packageTrees.Keys;

        public IEnumerable<Symbol> GlobalSymbols => PrimitiveSymbolTree.GlobalSymbols
                                                    .Concat(Packages.SelectMany(Children));
        public IEnumerable<Symbol> Symbols => packageTrees.Values.SelectMany(t => t.Symbols);

        public SymbolForest(PrimitiveSymbolTree primitiveSymbolTree, SymbolTreeBuilder symbolTreeBuilder, IEnumerable<FixedSymbolTree> packageTrees)
        {
            if (symbolTreeBuilder.Package is null)
                throw new ArgumentException("Can't be builder for primitive symbols", nameof(symbolTreeBuilder));
            this.PrimitiveSymbolTree = primitiveSymbolTree;
            this.packageTrees = packageTrees.Append<ISymbolTree>(symbolTreeBuilder).ToFixedDictionary(t => t.Package!);
        }

        public IEnumerable<Symbol> Children(Symbol symbol)
        {
            if (symbol.Package is null)
                return PrimitiveSymbolTree.Children(symbol);

            if (!packageTrees.TryGetValue(symbol.Package, out var tree))
                throw new ArgumentException("Symbol must be for one of the packages in this tree", nameof(symbol));

            return tree.Children(symbol);
        }
    }
}
