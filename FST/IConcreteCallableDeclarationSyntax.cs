using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(IConstructorDeclarationSyntax),
        typeof(IConcreteMethodDeclarationSyntax),
        typeof(IFunctionDeclarationSyntax),
        typeof(IAssociatedFunctionDeclarationSyntax))]
    public interface IConcreteCallableDeclarationSyntax : ICallableDeclarationSyntax
    {
        IBodySyntax Body { get; }
    }
}
