using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Symbols
{
    /// <summary>
    /// A symbol that has child symbols
    /// </summary>
    [Closed(
        typeof(TypeSymbol),
        typeof(FunctionSymbol),
        typeof(NamespaceSymbol))]
    public abstract class ParentSymbol : Symbol
    {
        public SymbolSet ChildSymbols { get; }

        private protected ParentSymbol(Name fullName, SymbolSet childSymbols)
            : base(fullName)
        {
            ChildSymbols = childSymbols;
        }
    }
}
