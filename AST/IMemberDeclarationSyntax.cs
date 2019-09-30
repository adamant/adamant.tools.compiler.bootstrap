using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IMethodDeclarationSyntax),
        typeof(IFieldDeclarationSyntax),
        typeof(IConstructorDeclarationSyntax))]
    public interface IMemberDeclarationSyntax : IEntityDeclarationSyntax
    {
        IClassDeclarationSyntax? DeclaringType { get; }
        SimpleName Name { get; }
    }
}
