using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IClassDeclarationSyntax : ITypeMetadata
    {
        void CreateDefaultConstructor(SymbolTreeBuilder symbolTree);
    }
}
