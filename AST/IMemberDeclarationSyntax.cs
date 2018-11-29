using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// Declarations that can be a member of a type declaration of some kind
    /// </summary>
    public interface IMemberDeclarationSyntax : IDeclarationSyntax, ISymbol
    {
    }
}
