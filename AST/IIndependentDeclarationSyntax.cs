using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// Basically, non-member, non-namespace declarations
    /// </summary>
    [Closed(
        typeof(IClassDeclarationSyntax))]
    public interface IIndependentDeclarationSyntax : IEntityDeclarationSyntax
    {

    }
}
