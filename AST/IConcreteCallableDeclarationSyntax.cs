using Adamant.Tools.Compiler.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IConstructorDeclarationSyntax),
        typeof(IConcreteMethodDeclarationSyntax),
        typeof(IFunctionDeclarationSyntax))]
    public interface IConcreteCallableDeclarationSyntax : ICallableDeclarationSyntax
    {
        FixedList<IStatementSyntax> Body { get; }
    }
}
