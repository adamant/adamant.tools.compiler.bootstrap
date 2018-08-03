using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public class NamespaceSyntaxSymbol : IDeclarationSyntaxSymbol
    {
        public string Name { get; }

        int? ISyntaxSymbol.DeclarationNumber => null;

        public IReadOnlyList<NamespaceSyntax> Declarations { get; }
        IEnumerable<DeclarationSyntax> IDeclarationSyntaxSymbol.Declarations => Declarations;
        IEnumerable<SyntaxBranchNode> ISyntaxSymbol.Declarations => Declarations;

        public IReadOnlyList<IDeclarationSyntaxSymbol> Children { get; }
        IEnumerable<ISyntaxSymbol> ISyntaxSymbol.Children => Children;

        public NamespaceSyntaxSymbol(IEnumerable<NamespaceSyntax> declarations, IEnumerable<IDeclarationSyntaxSymbol> children)
        {
            Declarations = declarations.ToList().AsReadOnly();
            Name = Declarations.First().Name.Value;
            Children = children.ToList().AsReadOnly();
        }
    }
}
