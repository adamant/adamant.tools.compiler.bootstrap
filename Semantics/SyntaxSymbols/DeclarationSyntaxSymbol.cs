using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public class DeclarationSyntaxSymbol : ISyntaxSymbol
    {
        public string Name { get; }
        private readonly HashSet<ISyntaxSymbol> children = new HashSet<ISyntaxSymbol>();
        public IEnumerable<ISyntaxSymbol> Children => children;

        private readonly IList<DeclarationSyntax> declarations;
        public IEnumerable<DeclarationSyntax> Declarations => declarations;
        IEnumerable<SyntaxNode> ISyntaxSymbol.Declarations => declarations;

        /// For variables and parameters, the type of the value they represent.
        /// For class/struct/enum symbols, the type being declared
        public readonly DataType Type;

        public DeclarationSyntaxSymbol(string name, DataType type, params DeclarationSyntax[] declarations)
        {
            Name = name;
            Type = type;
            this.declarations = declarations.ToList();
        }

        public void AddChild(ISyntaxSymbol child)
        {
            children.Add(child);
        }
    }
}
