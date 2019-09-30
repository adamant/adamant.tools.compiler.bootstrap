using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IFunctionDeclarationSyntax),
        typeof(IFieldDeclarationSyntax),
        typeof(IConstructorDeclarationSyntax))]
    public interface IMemberDeclarationSyntax : IDeclarationSyntax, ISymbol
    {
        ClassDeclarationSyntax? DeclaringType { get; }
    }
}
