using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// Things that can be declared outside of a class
    /// </summary>
    public partial interface INonMemberDeclarationSyntax
    {
        NamespaceOrPackageSymbol ContainingNamespaceSymbol { get; set; }
    }
}
