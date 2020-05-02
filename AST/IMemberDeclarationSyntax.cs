using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IMethodDeclarationSyntax),
        typeof(IFieldDeclarationSyntax),
        typeof(IConstructorDeclarationSyntax),
        typeof(IAssociatedFunctionDeclarationSyntax))]
    public interface IMemberDeclarationSyntax : IEntityDeclarationSyntax
    {
        IClassDeclarationSyntax DeclaringClass { get; }
        SimpleName Name { get; }
    }
}
