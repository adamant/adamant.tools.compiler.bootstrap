using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IClassDeclarationSyntax : IDeclarationSyntax, ITypeSymbol
    {
        FixedList<MemberDeclarationSyntax> Members { get; }
    }
}
