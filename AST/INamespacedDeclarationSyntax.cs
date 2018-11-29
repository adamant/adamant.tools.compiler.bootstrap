using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// Declarations that can be a member of a namespace. Except that namespace
    /// declarations are NOT included here.
    /// </summary>
    public interface INamespacedDeclarationSyntax : ISymbol, IDeclarationSyntax
    {
    }
}
