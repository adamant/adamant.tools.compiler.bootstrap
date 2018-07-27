using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    internal class NamespaceSyntaxSymbol : SyntaxSymbol<NamespaceSyntax>, INamespaceSyntaxSymbol
    {
        public NamespaceSyntaxSymbol(
            string name,
            NamespaceSyntax declaration)
            : base(name, declaration)
        {
        }
    }
}
