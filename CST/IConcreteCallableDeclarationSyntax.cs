using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(IConstructorDeclarationSyntax),
        typeof(IConcreteMethodDeclarationSyntax),
        typeof(IFunctionDeclarationSyntax),
        typeof(IAssociatedFunctionDeclarationSyntax))]
    public partial interface IConcreteCallableDeclarationSyntax : ICallableDeclarationSyntax
    {
        IBodySyntax Body { get; }
    }
}
