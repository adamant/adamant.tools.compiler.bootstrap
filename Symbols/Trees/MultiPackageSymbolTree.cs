using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Trees
{
    public class MultiPackageSymbolTree : SymbolTree
    {
        private readonly FixedDictionary<PackageSymbol, PackageSymbolTree> packageTrees;

        public MultiPackageSymbolTree(IEnumerable<PackageSymbolTree> packageTrees)
        {
            this.packageTrees = packageTrees.ToFixedDictionary(t => t.Package);
        }

        public override IEnumerable<Symbol> Children(Symbol symbol)
        {
            if (!packageTrees.TryGetValue(symbol.Package, out var tree))
                throw new ArgumentException("Symbol must be for one of the packages in this tree", nameof(symbol));

            return tree.Children(symbol);
        }
    }
}
