using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IClassDeclarationSyntax : INonMemberEntityDeclarationSyntax, ITypeSymbol
    {
        FixedList<IModiferToken> Modifiers { get; }
        SimpleName Name { get; }
        FixedList<IMemberDeclarationSyntax> Members { get; }
        new TypePromise DeclaresType { get; }
        void CreateDefaultConstructor();
    }
}
