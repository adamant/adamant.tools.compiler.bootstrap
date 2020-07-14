using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    /// <summary>
    /// While namespaces in syntax declarations are repeated across files, and
    /// IL doesn't even directly represent namespaces, for symbols, a namespace
    /// is the container of all the names in it. There is one symbol per namespace.
    /// </summary>
    public sealed class NamespaceSymbol : ParentSymbol
    {
        public NamespaceSymbol(Name fullName, SymbolSet childSymbols)
            : base(fullName, childSymbols) { }
    }
}
