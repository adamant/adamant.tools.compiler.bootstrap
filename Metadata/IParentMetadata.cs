using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    /// <summary>
    /// Metadata that has child metadata
    /// </summary>
    [Closed(
        typeof(ITypeMetadata),
        typeof(IFunctionMetadata),
        typeof(INamespaceMetadata))]
    public interface IParentMetadata : IMetadata
    {
        MetadataSet ChildMetadata { get; }
    }
}
