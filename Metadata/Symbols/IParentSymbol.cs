using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    /// <summary>
    /// A symbol that has child symbols
    /// </summary>
    [Closed(
        typeof(ITypeSymbol),
        typeof(IFunctionSymbol))]
    public interface IParentSymbol : ISymbol
    {
        SymbolSet ChildSymbols { get; }
    }
}
