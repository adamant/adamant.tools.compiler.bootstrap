using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// Basically, non-member, non-namespace declarations
    /// </summary>
    [Closed(
        typeof(IClassDeclarationSyntax),
        typeof(IFunctionDeclarationSyntax))]
    public interface IIndependentDeclarationSyntax : IEntityDeclarationSyntax
    {
    }
}
