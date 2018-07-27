using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    internal class GlobalNamespaceSyntaxSymbol : SyntaxSymbol<CompilationUnitSyntax>, IGlobalNamespaceSyntaxSymbol
    {
        public GlobalNamespaceSyntaxSymbol(
            PackageSyntax package)
            : base("")
        {
        }
    }
}
