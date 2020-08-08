using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols.Trees
{
    /// <summary>
    /// A symbol tree is a immutable collection of symbols that answers the question:
    /// For any given symbol, what are its child symbols.
    ///
    /// Each symbol tree 
    /// </summary>
    public abstract class SymbolTree
    {
        public PackageSymbol Package { get; }
        public abstract IEnumerable<Symbol> Symbols { get; }

        protected SymbolTree(PackageSymbol package)
        {
            Package = package;
        }

        public abstract IEnumerable<Symbol> Children(Symbol symbol);
    }
}
