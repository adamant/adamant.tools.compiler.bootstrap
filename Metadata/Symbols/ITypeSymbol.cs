using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    /// <summary>
    /// A symbol for a type declaration (i.e. a class)
    /// </summary>
    public interface ITypeSymbol : ISymbol
    {
        DataType DeclaresType { get; }
    }
}
