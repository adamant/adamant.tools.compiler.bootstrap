using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.SyntaxSymbols
{
    public class GlobalNamespaceSyntaxSymbol : ISyntaxSymbol
    {
        public string Name => "";

        int? ISyntaxSymbol.DeclarationNumber => null;

        public IReadOnlyList<CompilationUnitSyntax> Declarations { get; }
        IEnumerable<SyntaxNode> ISyntaxSymbol.Declarations => Declarations;

        public IReadOnlyList<IDeclarationSyntaxSymbol> Children { get; }
        IEnumerable<ISyntaxSymbol> ISyntaxSymbol.Children => Children;

        public GlobalNamespaceSyntaxSymbol(IEnumerable<CompilationUnitSyntax> declarations, IEnumerable<IDeclarationSyntaxSymbol> children)
        {
            Declarations = declarations.ToList().AsReadOnly();
            Children = children.ToList().AsReadOnly();
        }
    }
}
