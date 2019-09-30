using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// Base type for any declaration that declares a callable thing
    /// </summary>
    [Closed(
        typeof(IConstructorDeclarationSyntax),
        typeof(INamedFunctionDeclarationSyntax))]
    public interface ICallableDeclarationSyntax : IEntityDeclarationSyntax
    {

    }
}
