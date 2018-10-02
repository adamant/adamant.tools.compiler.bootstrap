using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public class TypeSyntaxSymbol : IDeclarationSyntaxSymbol
    {
        public string Name { get; }

        int? ISyntaxSymbol.DeclarationNumber => null;

        public DeclarationSyntax Declaration { get; }
        IEnumerable<DeclarationSyntax> IDeclarationSyntaxSymbol.Declarations => Declaration.Yield();
        IEnumerable<SyntaxBranchNode> ISyntaxSymbol.Declarations => Declaration.Yield();

        IEnumerable<ISyntaxSymbol> ISyntaxSymbol.Children => Enumerable.Empty<ISyntaxSymbol>();

        public TypeSyntaxSymbol(DeclarationSyntax declaration)
        {
            Name = declaration.Name.Value;
            Declaration = declaration;
        }
    }
}
