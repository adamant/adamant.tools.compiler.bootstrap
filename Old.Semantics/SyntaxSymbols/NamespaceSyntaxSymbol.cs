using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics.SyntaxSymbols
{
    public class NamespaceSyntaxSymbol : IDeclarationSyntaxSymbol
    {
        public string Name { get; }

        int? ISyntaxSymbol.DeclarationNumber => null;

        public IReadOnlyList<CompilationUnitNamespaceSyntax> Declarations { get; }
        IEnumerable<DeclarationSyntax> IDeclarationSyntaxSymbol.Declarations => throw new NotImplementedException();
        IEnumerable<SyntaxNode> ISyntaxSymbol.Declarations => Declarations;

        public IReadOnlyList<IDeclarationSyntaxSymbol> Children { get; }
        IEnumerable<ISyntaxSymbol> ISyntaxSymbol.Children => Children;

        public NamespaceSyntaxSymbol(IEnumerable<CompilationUnitNamespaceSyntax> declarations, IEnumerable<IDeclarationSyntaxSymbol> children)
        {
            Declarations = declarations.ToList().AsReadOnly();
            throw new NotImplementedException();
            //Name = Declarations.First().Name.Value;
            Children = children.ToList().AsReadOnly();
        }
    }
}
