using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    internal class Namespace : INamespaceSymbol
    {
        public Name FullName { get; }
        public SymbolSet ChildSymbols { get; }
        public SymbolSet NestedSymbols { get; }
        public Namespace(Name fullName, IEnumerable<ISymbol> childSymbols, IEnumerable<ISymbol> nestedSymbols)
        {
            FullName = fullName;
            ChildSymbols = new SymbolSet(childSymbols);
            NestedSymbols = new SymbolSet(nestedSymbols);
        }
    }
}
