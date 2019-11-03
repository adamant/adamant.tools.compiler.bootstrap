namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    /// <summary>
    /// While namespaces in syntax declarations are repeated across files, and
    /// IL doesn't even directly represent namespaces, for symbols, a namespace
    /// is the container of all the names in it. There is one symbol per namespace.
    /// </summary>
    public interface INamespaceSymbol : IParentSymbol
    {
    }
}
