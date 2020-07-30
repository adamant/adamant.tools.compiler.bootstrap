using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IClassDeclarationSyntax : INonMemberEntityDeclarationSyntax, ITypeSymbol
    {
        IMutableKeywordToken? MutableModifier { get; }
        SimpleName Name { get; }
        FixedList<IMemberDeclarationSyntax> Members { get; }
        new TypePromise DeclaresType { get; }
        void CreateDefaultConstructor();
    }
}
