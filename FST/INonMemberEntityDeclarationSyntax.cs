using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    /// <summary>
    /// Basically, non-member, non-namespace declarations
    /// </summary>
    [Closed(
        typeof(IClassDeclarationSyntax),
        typeof(IFunctionDeclarationSyntax))]
    public interface INonMemberEntityDeclarationSyntax : IEntityDeclarationSyntax, INonMemberDeclarationSyntax
    {
    }
}
