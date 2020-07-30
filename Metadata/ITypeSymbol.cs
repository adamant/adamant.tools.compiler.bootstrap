using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    /// <summary>
    /// A symbol for a type declaration (i.e. a class)
    /// </summary>
    public interface ITypeSymbol : IParentSymbol
    {
        DataType DeclaresType { get; }
    }
}
