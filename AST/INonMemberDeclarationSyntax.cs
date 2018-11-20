using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// Declarations that can be a member of a namespace. Note that namespace
    /// declarations are NOT included here.
    /// </summary>
    public interface INonMemberDeclarationSyntax : ISymbol
    {
    }
}
