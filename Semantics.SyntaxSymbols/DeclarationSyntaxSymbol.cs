using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    internal class DeclarationSyntaxSymbol : SyntaxSymbol<DeclarationSyntax>, IDeclarationSyntaxSymbol
    {
        public DeclarationSyntaxSymbol(
            string name,
            DeclarationSyntax declaration)
            : base(name, declaration)
        {
        }

        public DeclarationSyntaxSymbol(string name, int declarationNumber, DeclarationSyntax declaration)
            : base(name, declarationNumber, declaration)
        {
        }
    }
}
