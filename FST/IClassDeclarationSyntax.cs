using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IClassDeclarationSyntax : INonMemberEntityDeclarationSyntax, ITypeSymbol
    {
        SimpleName Name { get; }
        FixedList<IMemberDeclarationSyntax> Members { get; }
        new TypePromise DeclaresType { get; }
        void CreateDefaultConstructor();
    }
}
