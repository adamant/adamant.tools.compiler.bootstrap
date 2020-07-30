namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    /// <summary>
    /// While namespaces in syntax declarations are repeated across files, and
    /// IL doesn't even directly represent namespaces, for metadata, a namespace
    /// is the container of all the names in it. There is one metadata object per namespace.
    /// </summary>
    public interface INamespaceMetadata : IParentMetadata
    {
    }
}
