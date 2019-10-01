using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// Things that can be declared outside of a class
    /// </summary>
    [Closed(
        typeof(INamespaceDeclarationSyntax),
        typeof(INonMemberEntityDeclarationSyntax))]
    public interface INonMemberDeclarationSyntax : IDeclarationSyntax
    {
    }
}
