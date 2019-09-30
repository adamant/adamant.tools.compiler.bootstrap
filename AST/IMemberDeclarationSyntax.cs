using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IFunctionDeclarationSyntax),
        typeof(IFieldDeclarationSyntax),
        typeof(IConstructorDeclarationSyntax))]
    public interface IMemberDeclarationSyntax : IEntityDeclarationSyntax
    {
        ClassDeclarationSyntax? DeclaringType { get; }
    }
}
