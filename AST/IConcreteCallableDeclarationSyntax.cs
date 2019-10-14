using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IConstructorDeclarationSyntax),
        typeof(IConcreteMethodDeclarationSyntax),
        typeof(IFunctionDeclarationSyntax))]
    public interface IConcreteCallableDeclarationSyntax : ICallableDeclarationSyntax
    {
        IBodySyntax Body { get; }
    }
}
