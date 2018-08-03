using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public class FunctionSyntaxSymbol : IDeclarationSyntaxSymbol
    {
        public string Name { get; }

        int? ISyntaxSymbol.DeclarationNumber => null;

        public FunctionDeclarationSyntax Declaration { get; }
        IEnumerable<DeclarationSyntax> IDeclarationSyntaxSymbol.Declarations => Declaration.Yield();
        IEnumerable<SyntaxBranchNode> ISyntaxSymbol.Declarations => Declaration.Yield();

        public IReadOnlyList<VariableSyntaxSymbol> Children { get; }
        IEnumerable<ISyntaxSymbol> ISyntaxSymbol.Children => Children;

        public FunctionSyntaxSymbol(FunctionDeclarationSyntax declaration, IEnumerable<VariableSyntaxSymbol> children)
        {
            Name = declaration.Name.Value;
            Declaration = declaration;
            Children = children.ToList().AsReadOnly();
        }
    }
}
