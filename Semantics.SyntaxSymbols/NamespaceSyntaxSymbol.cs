using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    internal class NamespaceSyntaxSymbol : SyntaxSymbol<NamespaceSyntax>, INamespaceSyntaxSymbol
    {
        /// Creates a global namespace symbol
        public NamespaceSyntaxSymbol(
            PackageSyntax package)
        : base("", null)
        {

        }

        public NamespaceSyntaxSymbol(
            string name,
            NamespaceSyntax declaration)
            : base(name, declaration)
        {
        }
    }
}
