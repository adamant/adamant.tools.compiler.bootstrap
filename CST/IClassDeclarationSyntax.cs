using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IClassDeclarationSyntax : ITypeMetadata
    {
        ConstructorSymbol? CreateDefaultConstructor();
    }
}
