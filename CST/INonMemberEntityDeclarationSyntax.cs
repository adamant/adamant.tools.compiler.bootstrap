using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// Basically, non-member, non-namespace declarations
    /// </summary>
    [Closed(
        typeof(IClassDeclarationSyntax),
        typeof(IFunctionDeclarationSyntax))]
    public partial interface INonMemberEntityDeclarationSyntax : IEntityDeclarationSyntax, INonMemberDeclarationSyntax
    {
    }
}
