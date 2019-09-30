using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// All non-namespace declarations since a namespace doesn't really create
    /// a thing, it just defines a group of names.
    /// </summary>
    [Closed(
        typeof(IIndependentDeclarationSyntax),
        typeof(IMemberDeclarationSyntax),
        typeof(ICallableDeclarationSyntax))]
    public interface IEntityDeclarationSyntax : IDeclarationSyntax, ISymbol
    {

    }
}
