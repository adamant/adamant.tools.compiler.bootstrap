using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IClassDeclarationSyntax
    {
        void CreateDefaultConstructor(SymbolTreeBuilder symbolTree);
    }
}
