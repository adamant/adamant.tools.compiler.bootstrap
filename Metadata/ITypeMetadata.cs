using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    /// <summary>
    /// Metadata for a type declaration (i.e. a class)
    /// </summary>
    public interface ITypeMetadata : IParentMetadata
    {
        DataType DeclaresType { get; }
    }
}
