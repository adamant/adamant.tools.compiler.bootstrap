using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// Basically, non-member, non-namespace declarations
    /// </summary>
    public partial interface INonMemberEntityDeclarationSyntax
    {
        NamespaceOrPackageSymbol ContainingNamespaceSymbol { get; set; }
    }
}
